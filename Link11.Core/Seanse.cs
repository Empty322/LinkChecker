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
using Link11.Core;
using Logger;
using Newtonsoft.Json;


namespace Link11.Core
{
    public enum Mode
    {
        Clew,
        Slew
    }

    public struct Configuration
    {
        public double AbonentsK { get; set; }
        public double IntervalsK { get; set; }
    }

    public class Seanse : INotifyPropertyChanged
    {
        #region Events

        public Action<object, EventArgs> ActiveStart;

        public Action<object, EventArgs> WorkingStart;

        #endregion

        #region Properties

        public string Directory { get; private set; }
        
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
                if (signalEntries.Count == 0)
                    return "";
                return signalEntries.Where(x => x.Type != EntryType.Error).Last().Time.ToShortTimeString();
            }
        }
        public string LastActiveTime
        {
            get { 
                IEnumerable<SignalEntry> active = signalEntries.Where(x => x.Type != EntryType.Error && ((x.Size > 200 && Mode == Mode.Clew) ||  (x.Size > 400 && Mode == Mode.Slew)));
                if (active.Count() == 0)
                    return "";
                return active.Last().Time.ToShortTimeString();
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
                SeanseState result;
                if (signalEntries.Count != 0)
                {
                    DateTime lastEntryTime = signalEntries.Last().Time;
                    TimeSpan delay = lastModified - lastEntryTime;
                    DateTime timeWithoutDelay = DateTime.Now - delay;

                    result = SeanseState.WorkingLevel0;
                    List<SignalEntry> lastEntries = signalEntries.Where(x => x.Time > timeWithoutDelay - new TimeSpan(0, 3, 0) && x.Time < DateTime.Now - delay).ToList();
                    if (lastEntries.Where(x =>
                        x.Type != EntryType.Error &&
                        (
                            (Mode == Mode.Clew && (x.Size - x.Errors > 200)) ||
                            (Mode == Mode.Clew && (x.Size - x.Errors > 200))
                        )).Count() > 0)
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
                    else if (lastEntries.Count == 0)
                        result = SeanseState.WorkingLevel0;
                }
                else
                {
                    result = SeanseState.WorkingLevel0;
                }
                return result;
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


        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #endregion

        #region Fields

        public List<SignalEntry> signalEntries;
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
        private int edge;

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
            this.config = new Configuration { AbonentsK = 0.1, IntervalsK = 0.05 };
            this.logger = logger;
            this.signalEntries = new List<SignalEntry>();
            this.parser = parser;
            this.logger = logger;
            this.config = config;
            this.edge = 0;
            Update();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            logger.LogMessage(" UPDATING " + Directory, LogLevel.Info);

            SeanseState prevState = State;

            LoadSignal(Directory + "log.txt");

            try
            {
                string allLog;

                using (StreamReader fs = new StreamReader(Directory + "all_log.txt"))
                {
                    allLog = fs.ReadToEnd();
                }
                string mode = allLog.Substring(11, 4);
                if (mode == "CLEW")
                {
                    Mode = Mode.Clew;
                    edge = 200;
                }
                else if (mode == "SLEW")
                {
                    Mode = Mode.Slew;
                    edge = 400;
                }
                string freq = allLog.Substring(25, 6).Replace('.', ',');

                Freq = float.Parse(freq);



                Type df = this.GetType();
                foreach (PropertyInfo pi in df.GetProperties())
                    OnPropertyChanged(pi.Name);

                if (prevState != SeanseState.Active && State == SeanseState.Active)
                    ActiveStart.Invoke(this, new EventArgs());
                if (prevState == SeanseState.WorkingLevel0 && State != SeanseState.WorkingLevel0)
                    WorkingStart.Invoke(this, new EventArgs());
            }
            catch (Exception e)
            {
                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
            }
        }
        public void Copy(DirectoryInfo destination)
        {
            if (signalEntries.Count != 0)
            {
                logger.LogMessage(" COPYING " + Directory + " TO " + destination, LogLevel.Info);

                DirectoryInfo di = new DirectoryInfo(Directory);
                if (!di.Exists)
                {
                    logger.LogMessage("Copying directory " + di.FullName + " impossible. This directory does'n exists.", LogLevel.Warning);
                    return;
                }
                FileInfo[] files = di.GetFiles();
                if (!destination.Exists)
                    destination.Create();
                DirectoryInfo dest = new DirectoryInfo(destination.ToString() + '\\' + di.Name);
                dest.Create();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.CopyTo(dest.FullName + '\\' + file.Name, true);
                        logger.LogMessage("     COPYING FILE: " + file.FullName, LogLevel.Info);
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

        //public bool IsActive()
        //{
        //    if (signalEntries.Count == 0)
        //        return false;
        //    return IsActive(signalEntries.First().Time, signalEntries.Last().Time);
        //}

        //public bool IsActive(DateTime start, DateTime end)
        //{
        //    if (start > end)
        //    {
        //        return IsActive(start, DateTime.Parse("23:59:59")) || IsActive(DateTime.Parse("00:00"), end);
        //    }
        //    List<SignalEntry> messages = signalEntries.Where(x =>
        //        x.Time >= start &&
        //        x.Time < end &&
        //        x.Type != EntryType.Error).ToList();

        //    int edge = 0;
        //    switch (Mode)
        //    {
        //        case Mode.Clew:
        //            edge = 200;
        //            break;
        //        case Mode.Slew:
        //            edge = 400;
        //            break;
        //        default:
        //            break;
        //    }

        //    foreach (SignalEntry message in messages)
        //    {
        //        if (message.Size - message.Errors > edge)
        //            return true;
        //    }
        //    return false;
        //}

        //public bool IsWorking()
        //{
        //    if (signalEntries.Count == 0)
        //        return false;
        //    return IsWorking(signalEntries.First().Time, signalEntries.Last().Time);
        //}

        //public bool IsWorking(DateTime start, DateTime end)
        //{
        //    if (end < start)
        //    {
        //        return IsWorking(start, DateTime.Parse("23:59:59")) || IsWorking(DateTime.Parse("00:00"), end);
        //    }
        //    else
        //    {
        //        List<SignalEntry> entries = signalEntries.Where(x => x.Time >= start && x.Time < end).ToList();

        //        if (entries.Any())
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        #endregion

        #region Private Methods

        private void LoadSignal(List<string> lines)
        {
            signalEntries = parser.Parse(lines);
        }

        private void LoadSignal(string file)
        {
            try
            {
                FileInfo fi = new FileInfo(file);
                if (lastModified != fi.LastWriteTime)
                {
                    if (File.Exists(fi.DirectoryName + "\\temp.txt"))
                        File.Delete(fi.DirectoryName + "\\temp.txt");
                    File.Copy(file, fi.DirectoryName + "\\temp.txt");
                    List<string> lines = new List<string>();

                    using (FileStream fs = File.OpenRead(fi.DirectoryName + "\\temp.txt"))
                    {
                        lines.AddRange(File.ReadAllLines(fi.DirectoryName + "\\temp.txt", Encoding.Default));
                    }
                                                                                                             
                    File.Delete(fi.DirectoryName + "\\temp.txt");
                    LoadSignal(lines);
                    lastModified = fi.LastWriteTime;

                    logger.LogMessage(fi.FullName + " Successfully loaded", LogLevel.Info);

                    lastUpdate = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
            }
        }

        private List<int> GetAbonents()
        {
            if (signalEntries.Count != 0)
                return GetAbonents(signalEntries.First().Time, signalEntries.Last().Time);
            else
                return new List<int>();
        }

        private List<int> GetAbonents(DateTime start, DateTime end)
        {
            List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time <= end && x.Type != EntryType.Error).ToList();
            if (CurrentEntries.Count == 0)
                return new List<int>();
            Dictionary<int, int> abonentsEntries = new Dictionary<int, int>();
            List<int> abonents = new List<int>();
            if (start.Hour > end.Hour)
            {
                abonents.AddRange(GetAbonents(start, DateTime.Parse("23:59:59")));
                abonents.AddRange(GetAbonents(DateTime.Parse("00:00"), end));
            }
            else
            {
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
            return GetAbonentsWithInterval(signalEntries.First().Time, signalEntries.Last().Time, interval);
        }

        private List<int> GetAbonentsWithInterval(DateTime start, DateTime end, int interval)
        {
            List<int> abonents = GetAbonents(start, end);
            return GetAbonentsWithInterval(start, end, abonents, interval);
        }

        private List<int> GetAbonentsWithInterval(DateTime start, DateTime end, List<int> abonents, int interval)
        {
            List<int> result = new List<int>();
            List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end).ToList();
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
            return result;
        }

        private int GetMaxInFrames()
        {
            if (signalEntries.Count == 0)
                return 0;
            return GetMaxInFrames(signalEntries.First().Time, signalEntries.Last().Time);
        }

        private int GetMaxInFrames(DateTime start, DateTime end)
        {
            List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && x.Type != EntryType.Error).ToList();
            int[] sizeWihoutErrors = CurrentEntries.Select(x => x.Size - x.Errors).ToArray();

            int maxInFrames = 0;
            if (sizeWihoutErrors.Count() != 0)
                maxInFrames = sizeWihoutErrors.Max();
            if (maxInFrames < 0)
                maxInFrames = 0;

            return maxInFrames;
        }

        private float GetAverageSizeInFrames()
        {
            if (signalEntries.Count == 0)
                return 0;
            return GetAverageSizeInFrames(signalEntries.First().Time, signalEntries.Last().Time);
        }

        private float GetAverageSizeInFrames(DateTime start, DateTime end)
        {
            List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Time >= start && x.Time < end && (x.Type == EntryType.Message || x.Type == EntryType.Answer)).ToList();
            int[] sizeWihoutErrors = CurrentEntries.Select(x => x.Size - x.Errors).ToArray();

            float avgInFrames = 0;
            if (sizeWihoutErrors.Count() != 0)
                avgInFrames = (float)sizeWihoutErrors.Average();
            if (avgInFrames < 0)
                avgInFrames = 0;

            return avgInFrames;
        }

        private string GetIntervals()
        {
            List<SignalEntry> CurrentEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Ninterval != 0).ToList();
            string result = "";
            if (CurrentEntries.Count == 0)
                return result;
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
            return result;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }  

        #endregion   
    }
}
