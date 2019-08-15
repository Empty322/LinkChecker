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
    public class Seanse : INotifyPropertyChanged
    {
        #region Events

        public event Action<Seanse, EventArgs> ActiveStart = (sender, e) => { };
        public event Action<Seanse, EventArgs> ActiveEnd = (sender, e) => { };

        public event Action<Seanse, EventArgs> WorkingStart = (sender, e) => { };
        public event Action<Seanse, EventArgs> WorkingEnd = (sender, e) => { };


        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #endregion

        #region Properties

        public DirectoryInfo Directory { get; private set; }
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
            get
            {
                if (ActiveEntries.Any())
                    return ActiveEntries.Last().Split('\t')[0];
                else
                    return "";
            }
        }
        public string Intervals { 
            get {
                string result = "";
                Dictionary<int, int> intervalEntries = GetIntervals();
                int i = 0;
                while (i < intervalEntries.Count && i < 3)
                {
                    int max = intervalEntries.Max(x => x.Value);
                    result += intervalEntries.Where(x => x.Value == max).First() + " ";
                    intervalEntries.Remove(intervalEntries.Where(x => x.Value == max).First().Key);
                    i++;
                }
                return result; 
            } 
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
        public int MaxSize { get { return GetMaxInFrames(); } }
        public float MaxSizeInBytes { get { return (float)Math.Round(MaxSize * 3.75f, 2); } }
        public float AverageSizeInBytes { get { return (float)Math.Round(AverageSize * 3.75f, 2); } }
        public float AverageSize { get { return (float)Math.Round(GetAverageSizeInFrames()); } }
        public SeanseState State { get { return state; } }
        public List<string> ActiveEntries { get; set; }
        public string LastCopy { get { return lastCopy.ToShortTimeString(); } }
        public string LastUpdate {get { return lastUpdate.ToShortTimeString(); } }
        public bool Visible {
            get {
                if (config.HideEmptySeanses && !(signalEntries.Where(e => e.Type != EntryType.Error).Count() > config.Trashold))
                    return false;
                return true; 
            } 
        }
        public int PercentReceiving
        {
            get
            {
                int notErrorsCount = signalEntries.Where(x => x.Type != EntryType.Error).Count();
                if (notErrorsCount > 0)
                    return (int)Math.Round(100f / signalEntries.Count() * notErrorsCount);
                else
                    return 0;
            }
        }
        public List<TuningChartUnit> TuningChartUnits { get; set; }
        public List<WorkingChartUnit> WorkingChartUnits { get; set; }
        public List<SizeChartUnit> SizeChartUnits { get; set; }
        public List<AbonentInfo> Abonents { get; set; }

        private DateTime ServerTime
        {
            get
            {
                DateTime lastEntryTime = signalEntries.Last().Time;
                TimeSpan delay = lastModified - lastEntryTime;
                return DateTime.Now - delay;
            }
        }

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
        private bool isEnded;
        private bool isActiveEnded;

        // Исправить
        private const int countForWorkingLevel5 = 200;
        private const int countForWorkingLevel4 = 100;
        private const int countForWorkingLevel3 = 40;
        private const int countForWorkingLevel2 = 10;
        private const int countForWorkingLevel1 = 1;
        private const int kForWorkingChartWorkingLevel5 = 10002000;
        private const int kForWorkingChartWorkingLevel4 = 20004000;
        private const int kForWorkingChartWorkingLevel3 = 40008000;
        private const int kForWorkingChartWorkingLevel2 = 80016000;
        private const int kForWorkingChartWorkingLevel1 = 160032000;
                
        #endregion

        #region Ctor

        public Seanse(string directory, Configuration config) : this(directory, config, new Parser(), new PrimitiveLogger("log.txt", LogLevel.Error)) { }

        public Seanse(string directory, Configuration config, IParser parser, ILogger logger) 
        {
            this.Directory = new DirectoryInfo(directory);
            this.DirectoryExists = Directory.Exists;
            if (!DirectoryExists)
                throw new LogFileNotFoundException();
            this.ActiveEntries = new List<string>();
            this.TuningChartUnits = new List<TuningChartUnit>();
            this.WorkingChartUnits = new List<WorkingChartUnit>();
            this.Abonents = new List<AbonentInfo>();
            this.lastModified = new DateTime();
            this.logger = logger;
            this.signalEntries = new List<SignalEntry>();
            this.parser = parser;
            this.config = config;
            this.mode = Mode.Unknown;
            this.prevState = SeanseState.WorkingLevel0;
            this.state = SeanseState.WorkingLevel0;
            this.isEnded = true;
            this.isActiveEnded = true;
            Update();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // Если директория существует
            if (System.IO.Directory.Exists(Directory.FullName))
            {
                DirectoryExists = true;

                // Загрузить log.txt
                LoadAllLog();
                
                // Загрузить allLog.txt
                LoadLog();

                // Если это пустой сеанс, выйти
                if (signalEntries.Any())
                {
                    // Узнать состояние сеанса
                    state = GetState();

                    FireEvents();
                                        
                    // Получить данные для графика расстройки
                    TuningChartUnits = GetTuningChartUnits(config.SmoothValue);

                    // Получить данные для графика объема
                    SizeChartUnits = GetSizeChartUnits();

                    // Получить данные для графика работы
                    WorkingChartUnits = GetWorkingChartUnits(new TimeSpan(0,1,0));

                    // Получить вхождения с объемом, превышающим норму
                    if (signalEntries.Any())
                    {
                        ActiveEntries = signalEntries
                            .Where(x => x.Type != EntryType.Error && ((x.Size - x.Errors) > (int)Mode))
                            .Select(x => x.Time.ToShortTimeString() + '\t' + (x.Size - x.Errors))
                            .ToList();
                    }

                    UpdateAbonentsInfo(Abonents);

                    prevState = state;
                    lastModified = File.GetLastWriteTime(Directory + "\\log.txt");
                    lastUpdate = DateTime.Now;

                    Type df = this.GetType();
                    foreach (PropertyInfo pi in df.GetProperties())
                        OnPropertyChanged(pi.Name);
                }
            }
            else
            {
                DirectoryExists = false;
            }            
        }

        public bool Copy(DirectoryInfo destination)
        {
            bool result = false;
            // Если есть вхождения
            if (signalEntries.Count != 0)
            {
                // Если директория существует
                if (Directory.Exists)
                {
                    directoryExists = true;
                    // Если размер лога больше 40000 байт
                    FileInfo logInfo = new FileInfo(Directory + "/log.txt");
                    if (logInfo.Length >= config.CopyLengthTrashold && PercentReceiving >= config.CopyPercentTrashold) {
                        // Если есть изменения в log.txt
                        if (lastModified != Directory.LastWriteTime)
                        {
                            // Получить файлы сеанса
                            FileInfo[] files = Directory.GetFiles();

                            // Создать папку назначения, если ее нет
                            if (!destination.Exists)
                                destination.Create();
                            DirectoryInfo dest = new DirectoryInfo(destination.ToString() + '\\' + Directory.Name);
                            dest.Create();

                            // Скопировать файлы сеанса в папку назначения
                            foreach (FileInfo file in files)
                            {
                                try
                                {
                                    file.CopyTo(dest.FullName + '\\' + file.Name, true);
                                    if (file.Name == "log.txt")
                                    {
                                        lastCopy = DateTime.Now;
                                        result = true;
                                    }
                                }
                                catch (Exception e)
                                {
                                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                                }
                            }
                        }
                    }
                }
                else
                {
                    directoryExists = false;
                }
            }
            return result;
        }

        public void Delete()
        {
            Directory.Delete(true);
            DirectoryExists = false;
        }

        #endregion

        #region LogFileNotFoundException

        public class LogFileNotFoundException : FileNotFoundException
        {
            public LogFileNotFoundException() { }
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
                return;
            }
            catch (DirectoryNotFoundException)
            {
                logger.LogMessage("Папка c сеансом " + Directory + "не найдена", LogLevel.Warning);
                return;
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
                catch (FileNotFoundException)
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
                List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && (x.Type == EntryType.Message || x.Type == EntryType.Answer)).ToList();
                int[] sizeWihoutErrors = CurrentEntries.Select(x => x.Size - x.Errors).ToArray();

                if (sizeWihoutErrors.Count() != 0)
                    avgInFrames = (float)sizeWihoutErrors.Average();
                if (avgInFrames < 0)
                    avgInFrames = 0;
            }
            return avgInFrames;
        }

        private Dictionary<int, int> GetIntervals(params int[] abonents)
        {
            Dictionary<int, int> intervalEntries = new Dictionary<int, int>();
            if (signalEntries.Count != 0) {
                List<SignalEntry> currentEntries;
                if (abonents.Any())
                    currentEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Ninterval != 0 && (x.Abonent.HasValue  ? abonents.Contains(x.Abonent.Value) : false)).ToList();
                else
                    currentEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Ninterval != 0).ToList();

                if (currentEntries.Count != 0)
                {
                    List<int> intervals = currentEntries.Select(x => x.Ninterval).Distinct().ToList();
                    intervals.Sort();

                    foreach (int interval in intervals)
                    {
                        int count = currentEntries.Where(x => x.Ninterval == interval).Count();
                        intervalEntries.Add(interval, count);
                    }
                }
            }                 
            return intervalEntries;
        }

        private void UpdateAbonentsInfo(List<AbonentInfo> abonentsInfo)
        {
            List<int> abonents = GetAbonents();
            foreach (int abonent in abonents)
            {
                Dictionary<int, int> allAbonentIntervals = GetIntervals(abonent);
                Dictionary<int, int> selectedAbonentIntervals = new Dictionary<int, int>();
                int i = 0;
                while (i < allAbonentIntervals.Count && i < 3)
                {
                    int max = allAbonentIntervals.Max(x => x.Value);
                    KeyValuePair<int, int> interval = allAbonentIntervals.Where(x => x.Value == max).First();
                    selectedAbonentIntervals.Add(interval.Key, interval.Value);
                    allAbonentIntervals.Remove(interval.Key);
                    i++;
                }
                selectedAbonentIntervals.Reverse();

                AbonentInfo info = abonentsInfo.FirstOrDefault(abonentInfo => abonentInfo.Name == abonent);
                if (info == null)
                {
                    info = new AbonentInfo(abonent);
                    abonentsInfo.Add(info);
                }
                info.Count = signalEntries.Where(e => e.Abonent.HasValue && e.Abonent.Value == abonent).Count();
                info.UpdateIntervals(selectedAbonentIntervals);
            }
            Abonents = abonentsInfo.OrderByDescending(x => x.Count).ToList();
        }

        private SeanseState GetState() {
            SeanseState result = SeanseState.WorkingLevel0;

            if (signalEntries.Count != 0)
            {
                List<SignalEntry> lastEntries = signalEntries.Where(x => x.Time > ServerTime - new TimeSpan(0, 3, 0) && x.Time < ServerTime).ToList();
                if (lastEntries.Where(x => x.Type != EntryType.Error && ((x.Size - x.Errors) > (int)Mode)).Any())
                    result = SeanseState.Active;
                else if (lastEntries.Count >= countForWorkingLevel5)
                    result = SeanseState.WorkingLevel5;
                else if (lastEntries.Count >= countForWorkingLevel4)
                    result = SeanseState.WorkingLevel4;
                else if (lastEntries.Count >= countForWorkingLevel3)
                    result = SeanseState.WorkingLevel3;
                else if (lastEntries.Count >= countForWorkingLevel2)
                    result = SeanseState.WorkingLevel2;
                else if (lastEntries.Count >= 1)
                    result = SeanseState.WorkingLevel1;
            }
            return result;
        }

        private void FireEvents()
        {
            // Eсли перешел в активный
            if (isActiveEnded && prevState != SeanseState.Active && state == SeanseState.Active)
            {
                isActiveEnded = false;
                ActiveStart.Invoke(this, new EventArgs());
            }

            // Если Вышел из активного
            IEnumerable<SignalEntry> activeEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Size - x.Errors > (int)Mode);
            if (!isActiveEnded && activeEntries.Any() && !(activeEntries.Last().Time > ServerTime.AddMinutes(-config.MinutesToAwaitAfterEnd) && activeEntries.Last().Time <= ServerTime))
            {
                isActiveEnded = true;
                ActiveEnd.Invoke(this, new EventArgs());
            }

            // Если начал работу
            if (isEnded && state != SeanseState.WorkingLevel0)
            {
                isEnded = false;
                WorkingStart.Invoke(this, new EventArgs());
            }
            // Если окончил работу
            if (!isEnded && signalEntries.Where(x => x.Time > ServerTime.AddMinutes(-config.MinutesToAwaitAfterEnd) && x.Time <= ServerTime).Count() == 0)
            {
                isEnded = true;
                WorkingEnd.Invoke(this, new EventArgs());
            }
        }

        private List<TuningChartUnit> GetTuningChartUnits(int counterMax)
        {
            float avgTuning = signalEntries.Select(x => x.Tuning).Average();
            List<TuningChartUnit> units = new List<TuningChartUnit>();
            if (signalEntries.Count != 0)
            {
                List<SignalEntry>.Enumerator enumerator = signalEntries.GetEnumerator();
                List<SignalEntry> valuesToSmooth = new List<SignalEntry>();
                int counter = 0;
                while (enumerator.MoveNext() || valuesToSmooth.Any())
                {
                    if (enumerator.Current != null && counter < counterMax)
                    {
                        valuesToSmooth.Add(enumerator.Current);
                        counter++;
                    }
                    else if (counter >= counterMax || enumerator.Current == null)
                    {
                        TuningChartUnit unit = new TuningChartUnit();
                        for (int i = 0; i < valuesToSmooth.Count; i++)
                            if (valuesToSmooth[i].Tuning > (avgTuning + 50) || valuesToSmooth[i].Tuning < (avgTuning - 50))
                                valuesToSmooth[i].Tuning = avgTuning;
                        unit.Tuning = valuesToSmooth.Select(x => x.Tuning).Average();
                        unit.Time = valuesToSmooth.First().Time;
                        units.Add(unit);
                        valuesToSmooth.Clear();
                        counter = 0;
                    }
                }
                enumerator.Dispose();
            }
            return units;
        }

        private List<SizeChartUnit> GetSizeChartUnits() {
            List<SizeChartUnit> units = new List<SizeChartUnit>();
            if (signalEntries.Count != 0)
            {
                foreach (SignalEntry entry in signalEntries)
                {
                    SizeChartUnit unit = new SizeChartUnit();
                    unit.Time = entry.Time;
                    if (entry.Type != EntryType.Error)
                    {
                        int actualSize = entry.Size - entry.Errors;
                        unit.Size = actualSize < 0 ? 0 : actualSize;
                    }
                    else
                    {
                        unit.Size = 0;
                    }
                    units.Add(unit);
                }
            }
            return units;
        }

        private List<WorkingChartUnit> GetWorkingChartUnits(TimeSpan smoothTime)
        {
            List<WorkingChartUnit> units = new List<WorkingChartUnit>();
            List<SignalEntry> valuesToUnite = new List<SignalEntry>();
            if (signalEntries.Any())
            {
                DateTime end = signalEntries.Last().Time;
                DateTime currentTime = signalEntries.First().Time;
                do
                {

                    valuesToUnite = signalEntries.Where(x => x.Time > currentTime && x.Time < (currentTime + smoothTime)).ToList();
                    WorkingChartUnit newUnit = new WorkingChartUnit();
                    if (valuesToUnite.Where(x => x.Type != EntryType.Error && (x.Size - x.Errors) > (int)Mode).Count() > 0) {
                        newUnit.Time = currentTime;
                        newUnit.State = 2;
                    }
                    else if (valuesToUnite.Count >= 1) {
                        newUnit.Time = currentTime;
                        newUnit.State = 1;
                    }
                    else {
                        newUnit.Time = currentTime;
                        newUnit.State = 0;
                    }
                    currentTime += smoothTime;
                    if (currentTime.Hour < signalEntries.First().Time.Hour)
                        newUnit.Time.AddDays(1);
                    units.Add(newUnit);
                }
                while (currentTime < end);
            }
            return units;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion   
   
    }
}
