using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using Link11.Core.Interfaces;
using Link11.Core.Enums;
using Link11.Core.Charting;
using Link11.Core;
using Logger;
using Newtonsoft.Json;


namespace Link11.Core
{
    public struct Configuration
    {
        public double AbonentsK { get; set; }
        public double IntervalsK { get; set; }
    }

    public class Seanse : INotifyPropertyChanged
    {
        #region Events

        public event Action<object, EventArgs> ActiveStart = (sender, e) => { };

        public event Action<object, EventArgs> WorkingStart = (sender, e) => { };

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #endregion

        #region Properties

        public string Directory { get; private set; }
        public bool DirectoryExists
        {
            get
            {
                return directoryExists;
            }
            set
            {
                directoryExists = value;
                OnPropertyChanged("DirectoryExists");
            }
        }
        public float Freq {
            get { return freq; }
            private set
            {
                freq = value;
                OnPropertyChanged("Freq");
            }
        }
        public Mode Mode {
            get { return mode; }
            private set {
                mode = value;
                OnPropertyChanged("Mode");
            } }
        public string StartWorkingTime
        {
            get {
                if (signalEntries.Count != 0)
                    return signalEntries.First().Time.ToShortTimeString();
                else
                    return "";
            }
        }
        public string LastWorkingTime
        {
            get {
                IEnumerable<SignalEntry> withoutErrors = signalEntries.Where(x => x.Type != EntryType.Error).ToList();
                if (withoutErrors.Count() != 0)
                    return withoutErrors.Last().Time.ToShortTimeString();
                return "";
            }
        }
        public string LastActiveTime
        {
            get { 
                IEnumerable<SignalEntry> active = signalEntries.Where(x => x.Type != EntryType.Error && ((x.Size - x.Errors) > (int)Mode));
                if (active.Count() != 0)
                    return active.Last().Time.ToShortTimeString();
                return "";
            }
        }
        public int AbonentsCount {
            get { return GetAbonents().Count; }
        }
        public string Intervals {
            get { return GetIntervals(); }
        }
        public string Position
        {
            get { return position; }
            private set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }
        public string Coordinates
        {
            get { return coordinates; }
            private set
            {
                coordinates = value;
                OnPropertyChanged("Coordinates");
            }
        }
        public int MaxSize
        {
            get { return GetMaxInFrames(); }
        }
        public float MaxSizeInBytes
        {
            get { return (float)Math.Round(MaxSize * 3.75, 2); }
        }
        public float AverageSize
        {
            get { return (float)Math.Round(GetAverageSizeInFrames()); }
        }
        public SeanseState State
        {
            get
            {
                return state;
            }
        }
        public string LastCopy { 
            get {
                return lastCopy.ToShortTimeString();
            }
        }
        public string LastUpdate
        {
            get
            {
                return lastUpdate.ToShortTimeString();
            }
        }

        public List<TuningChartUnit> TuningChartUnits { get; set; }

        #endregion

        #region Fields

        public List<SignalEntry> signalEntries;
        private bool directoryExists;
        private float freq;
        private Mode mode;
        private string position;
        private string coordinates;
        private DateTime lastModified;
        private DateTime lastCopy;
        private DateTime lastUpdate;

        private ILogger logger;   
        private Configuration config;
        private IParser parser;
        private SeanseState prevState;
        private SeanseState state;

        private const int countForWorkingLevel5 = 200;
        private const int countForWorkingLevel4 = 100;
        private const int countForWorkingLevel3 = 40;
        private const int countForWorkingLevel2 = 10;
        private const int countForWorkingLevel1 = 1;
        
        #endregion

        #region Ctor

        public Seanse(string directory) : this(directory, new PrimitiveLogger("log.txt", LogLevel.Error)) { }

        public Seanse(string directory, ILogger logger) : this(directory, new Configuration{ AbonentsK = 0.15, IntervalsK = 0.2}, new Parser(), logger) { }

        public Seanse(string directory, Configuration config, IParser parser, ILogger logger) 
        {
            this.Directory = directory;
            this.DirectoryExists = new DirectoryInfo(directory).Exists;
            this.logger = logger;
            this.signalEntries = new List<SignalEntry>();
            this.parser = parser;
            this.config = config;
            this.mode = Mode.Unknown;
            this.prevState = SeanseState.WorkingLevel0;
            this.state = SeanseState.WorkingLevel0;
            Update();
        }

        #endregion

        #region Public Methods

        public void Copy(DirectoryInfo destination)
        {
            // Если есть вхождения
            if (signalEntries.Count != 0)
            {
                DirectoryInfo seanseDir = new DirectoryInfo(Directory);
                // Если директория существует
                if (seanseDir.Exists)
                {
                    directoryExists = true;
                    // Если есть изменения в log.txt
                    if (lastModified != seanseDir.LastWriteTime)
                    {
                        // Получить файлы сеанса
                        FileInfo[] files = seanseDir.GetFiles();

                        // Создать папку назначения, если ее нет
                        if (!destination.Exists)
                            destination.Create();
                        DirectoryInfo dest = new DirectoryInfo(destination.ToString() + '\\' + seanseDir.Name);
                        dest.Create();

                        // Скопировать файлы сеанса в папку назначения
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                file.CopyTo(dest.FullName + '\\' + file.Name, true);
                                if (file.Name == "log.txt")
                                    lastCopy = DateTime.Now;
                            }
                            catch (Exception e)
                            {
                                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                            }
                        }
                    }
                }
                else
                {
                    directoryExists = false;
                }

                
            }
        }

        public void Update()
        {
            DirectoryInfo di = new DirectoryInfo(Directory);
            // Если директория существует
            if (di.Exists)
            {
                DirectoryExists = true;

                // Загрузить log.txt
                LoadAllLog();
                // Загрузить allLog.txt
                LoadLog();

                // Узнать состояние сеанса
                state = GetState();

                // Eсли перешел в активный
                if (prevState != SeanseState.Active && state == SeanseState.Active)
                    ActiveStart.Invoke(this, new EventArgs());

                // Если начал работу
                if (state != SeanseState.WorkingLevel0 && lastModified < (DateTime.Now - new TimeSpan(0, 30, 0)))
                    WorkingStart.Invoke(this, new EventArgs());

                TuningChartUnits = GetTuningChartUnits();

                prevState = state;
                lastUpdate = DateTime.Now;
                lastModified = File.GetLastWriteTime(Directory + "\\log.txt");

                Type df = this.GetType();
                foreach (PropertyInfo pi in df.GetProperties())
                    OnPropertyChanged(pi.Name);
            }
            else
            {
                DirectoryExists = false;
            }            
        }

        #endregion

        #region Private Methods

        private void LoadAllLog()
        {
            FileInfo allLogFile = new FileInfo(Directory + "\\all_log.txt");

            string allLogFileContent = "";
            try
            {
                using (StreamReader fs = new StreamReader(allLogFile.FullName))
                {
                    // Попробовать прочитать файл
                    allLogFileContent = fs.ReadToEnd();
                }
            }
            catch (FileNotFoundException)
            {
                // Если его нет, сделать запись в логе приложения
                logger.LogMessage("Файл " + Directory + "\\all_log.txt не найден.", LogLevel.Warning);
            }

            // Попробовать парсить allLog.txt
            try
            {
                parser.ParseAllLog(allLogFileContent, out freq, out mode);            
            }            
            catch
            {
                // Если не удалось, сделать запись в логе приложения
                logger.LogMessage("Не удалось парсить файл " + allLogFile.FullName, LogLevel.Warning);
            }
        }

        private void LoadLog()
        {
            FileInfo fi = new FileInfo(Directory + "\\log.txt");
                              
            // Если файл был изменен
            if (lastModified != fi.LastWriteTime)
            {
                // Скопировать log.txt во временный файл 
                if (File.Exists(fi.DirectoryName + "\\temp.txt"))
                    File.Delete(fi.DirectoryName + "\\temp.txt");
                try
                {
                    File.Copy(fi.FullName, fi.DirectoryName + "\\temp.txt");
                }
                catch (FileNotFoundException e)
                {
                    throw new LogFileNotFoundException();
                }

                // Прочитать временный файл с логом
                List<string> lines = new List<string>();
                lines.AddRange(File.ReadAllLines(fi.DirectoryName + "\\temp.txt", Encoding.Default));                                                                                                             
                // Удалить временный файл
                File.Delete(fi.DirectoryName + "\\temp.txt");

                // Парсить log.txt
                signalEntries = parser.ParseLog(lines);
            }    
        }

        private List<int> GetAbonents()
        {
            List<int> abonents = new List<int>();
            if (signalEntries.Count != 0)
                abonents = GetAbonents(signalEntries.First().Time, signalEntries.Last().Time);
            return abonents;
        }

        private List<int> GetAbonents(DateTime start, DateTime end)
        {
            List<int> abonents = new List<int>();
            if (signalEntries.Count != 0)
            {
                Dictionary<int, int> abonentsEntries = new Dictionary<int, int>();
                if (start.Hour > end.Hour)
                {
                    abonents.AddRange(GetAbonents(start, DateTime.Parse("23:59:59")));
                    abonents.AddRange(GetAbonents(DateTime.Parse("00:00"), end));
                }
                else
                {
                    List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && x.Type != EntryType.Error).ToList();
                    if (CurrentEntries.Count == 0)
                        return new List<int>();
                    abonents = CurrentEntries.Where(x => x.Abonent.HasValue).Select(x => x.Abonent.Value).Distinct().ToList();
                    foreach (int abonent in abonents)
                    {
                        int count = CurrentEntries.Where(x => x.Abonent == abonent).Count();
                        abonentsEntries.Add(abonent, count);
                    }
                    if (abonents.Count() > 0)
                    {
                        int max = abonentsEntries.Max(x => x.Value);
                        int i = 0;
                        while (i < abonents.Count)
                        {
                            if (abonentsEntries[abonents[i]] < max * config.AbonentsK)
                            {
                                abonents.RemoveAt(i);
                                continue;
                            }
                            i++;
                        }
                    }
                }
            }
            return abonents;
        }

        private List<int> GetAbonentsWithInterval(int interval)
        {
            List<int> abonetns = new List<int>();
            if (signalEntries.Count != 0)
                abonetns = GetAbonentsWithInterval(signalEntries.First().Time, signalEntries.Last().Time, interval);
            return abonetns;
        }

        private List<int> GetAbonentsWithInterval(DateTime start, DateTime end, int interval)
        {
            List<int> abonents = GetAbonents(start, end);
            return GetAbonentsWithInterval(start, end, abonents, interval);
        }

        private List<int> GetAbonentsWithInterval(DateTime start, DateTime end, List<int> abonents, int interval)
        {
            List<int> result = new List<int>();
            if (signalEntries.Count != 0)
            {
                if (start.Hour > end.Hour)
                {
                    result.AddRange(GetAbonentsWithInterval(start, DateTime.Parse("23:59:59"), abonents, interval));
                    result.AddRange(GetAbonentsWithInterval(DateTime.Parse("00:00"), end, abonents, interval));
                }
                List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && x.Type != EntryType.Error && x.Ninterval != 0).ToList();
                CurrentEntries = CurrentEntries.Where(x => x.Abonent.HasValue && abonents.Contains(x.Abonent.Value)).ToList();

                Dictionary<int, int> intervalCount = new Dictionary<int, int>();
                foreach (int abonent in abonents)
                {
                    List<int> intervals = CurrentEntries.Where(x => x.Abonent == abonent && x.Ninterval != 0).Select(x => x.Ninterval).ToList();
                    int count = intervals.Where(x => x == interval).Count();
                    intervals = intervals.Where(x => x != interval).ToList();
                    logger.LogMessage("Абонент: " + abonent, LogLevel.Info);
                    logger.LogMessage("Количество" + interval + "интервалов = " + count, LogLevel.Info);
                    logger.LogMessage("Количество остальных интервалов = " + intervals.Count(), LogLevel.Info);
                    if (count > intervals.Count() * config.IntervalsK)
                        result.Add(abonent);
                }
            }
            return result;
        }

        private int GetMaxInFrames()
        {
            int maxInFrames = 0;
            if (signalEntries.Count != 0)
                maxInFrames = GetMaxInFrames(signalEntries.First().Time, signalEntries.Last().Time);
            return maxInFrames;
        }

        private int GetMaxInFrames(DateTime start, DateTime end)
        {
            int maxInFrames = 0;
            if (signalEntries.Count != 0)
            {
                if (start.Hour > end.Hour)
                {
                    maxInFrames += GetMaxInFrames(start, DateTime.Parse("23:59:59"));
                    maxInFrames += GetMaxInFrames(DateTime.Parse("00:00"), end);
                }
                List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && x.Type != EntryType.Error).ToList();
                int[] sizeWihoutErrors = CurrentEntries.Select(x => x.Size - x.Errors).ToArray();

                if (sizeWihoutErrors.Count() != 0)
                    maxInFrames = sizeWihoutErrors.Max();
                if (maxInFrames < 0)
                    maxInFrames = 0;
            }
            return maxInFrames;
        }

        private float GetAverageSizeInFrames()
        {
            float avgInFrames = 0;
            if (signalEntries.Count != 0)
                avgInFrames = GetAverageSizeInFrames(signalEntries.First().Time, signalEntries.Last().Time);
            return avgInFrames;
        }

        private float GetAverageSizeInFrames(DateTime start, DateTime end)
        {
            float avgInFrames = 0;
            if (signalEntries.Count != 0)
            {
                if (start.Hour > end.Hour)
                {
                    avgInFrames += GetAverageSizeInFrames(start, DateTime.Parse("23:59:59"));
                    avgInFrames += GetAverageSizeInFrames(DateTime.Parse("00:00"), end);
                }
                List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && (x.Type == EntryType.Message || x.Type == EntryType.Answer)).ToList();
                int[] sizeWihoutErrors = CurrentEntries.Select(x => x.Size - x.Errors).ToArray();

                if (sizeWihoutErrors.Count() != 0)
                    avgInFrames = (float)sizeWihoutErrors.Average();
                if (avgInFrames < 0)
                    avgInFrames = 0;
            }
            return avgInFrames;
        }

        private string GetIntervals()
        {
            string result = "";
            if (signalEntries.Count != 0) {
                List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Ninterval != 0).ToList();
                if (CurrentEntries.Count != 0)
                {
                    List<int> intervals = CurrentEntries.Select(x => x.Ninterval).Distinct().ToList();
                    intervals.Sort();

                    Dictionary<int, int> intervalEntries = new Dictionary<int, int>();
                    foreach (int interval in intervals)
                    {
                        int count = CurrentEntries.Where(x => x.Ninterval == interval).Count();
                        intervalEntries.Add(interval, count);
                    }
                    int i = 0;
                    while (i < intervalEntries.Count && i < 3)
                    {
                        int max = intervalEntries.Max(x => x.Value);
                        result += intervalEntries.Where(x => x.Value == max).First() + " ";
                        intervalEntries.Remove(intervalEntries.Where(x => x.Value == max).First().Key);
                        i++;
                    }
                }
            }                 
            return result;
        }

        private SeanseState GetState() {
            SeanseState result = SeanseState.WorkingLevel0;
            if (signalEntries.Count != 0)
            {
                DateTime lastEntryTime = signalEntries.Last().Time;
                TimeSpan delay = lastModified - lastEntryTime;
                DateTime timeWithoutDelay = DateTime.Now - delay;

                List<SignalEntry> lastEntries = signalEntries.Where(x => x.Time > timeWithoutDelay - new TimeSpan(0, 3, 0) && x.Time < DateTime.Now - delay).ToList();
                if (lastEntries.Where(x => x.Type != EntryType.Error && ((x.Size - x.Errors) > (int)Mode)).Count() > 0)
                    result = SeanseState.Active;
                else if (lastEntries.Count >= countForWorkingLevel5)
                    result = SeanseState.WorkingLevel5;
                else if (lastEntries.Count >= countForWorkingLevel4)
                    result = SeanseState.WorkingLevel4;
                else if (lastEntries.Count >= countForWorkingLevel3)
                    result = SeanseState.WorkingLevel3;
                else if (lastEntries.Count >= countForWorkingLevel2)
                    result = SeanseState.WorkingLevel2;
                else if (lastEntries.Count >= countForWorkingLevel1)
                    result = SeanseState.WorkingLevel1;
            }
            return result;
        }

        public List<TuningChartUnit> GetTuningChartUnits()
        {
            List<TuningChartUnit> units = new List<TuningChartUnit>();
            if (signalEntries.Count != 0)
            {
                float avg = signalEntries.Select(x => x.Tuning).Average();
                foreach (SignalEntry se in signalEntries)
                {
                    TuningChartUnit unit = new TuningChartUnit();
                    unit.Time = se.Time;
                    unit.Tuning = se.Tuning;
                    units.Add(unit);
                }
            }
            return units;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public class LogFileNotFoundException : FileNotFoundException
        {
            public LogFileNotFoundException() { }
        }

        #endregion   
   
    }
}
