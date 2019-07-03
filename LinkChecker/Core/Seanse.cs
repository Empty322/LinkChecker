using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11Core;
using System.IO;
using Link11Checker.ViewModels.Base;

namespace Link11Checker.Core
{
    public enum Mode
    {
        Clew,
        Slew
    }

    public class Seanse : BaseViewModel
    {
        #region Properties
        public Signal Signal { get { return signal; } }
        public string Directory { get; private set; }
        public string LastActiveTime
        {
            get { return lastActiveTime.ToShortTimeString(); }
            private set
            {
                lastActiveTime = DateTime.Parse(value);
                OnPropertyChanged("LastActiveTime");
            }
        }
        public string LastWorkingTime
        {
            get { return lastWorkingTime.ToShortTimeString(); }
            private set
            {
                lastWorkingTime = DateTime.Parse(value);
                OnPropertyChanged("LastWorkingTime");
            }
        }
        public string Position {
            get { return position; }
            private set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }
        public string Coordinates { get; set; }
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

        public int MaxSize {
            get { return maxSize; }
            set {
                maxSize = value;
                OnPropertyChanged("MaxSize");
            } }

        public float AverageSize {
            get { return (float)Math.Round(averageSize, 2); }
            set
            {
                averageSize = value;
                OnPropertyChanged("AverageSize");
            } }

        #endregion

        #region fields

        private readonly Signal signal;
        private DateTime lastActiveTime;
        private DateTime lastWorkingTime;
        private string position;
        private string coordinates;
        private float freq;
        private Mode mode;
        private int maxSize;
        private float averageSize;
        private int edge;
        private List<SignalEntry> activeEntries;

        #endregion

        #region Ctor

        public Seanse(string directory) 
        {
            Directory = directory;
            activeEntries = new List<SignalEntry>();
            signal = new Signal(new Parser(), new Configuration { AbonentsK = 0.1, IntervalsK = 0.05 });
            Update();
        }

        #endregion

        #region Methods

        public void Update()
        {
            signal.LoadSignal(Directory + "log.txt");
            string allLog;
            using (StreamReader fs = new StreamReader(Directory + "all_log.txt"))
            {
                allLog = fs.ReadToEnd();
            }
            string mode = allLog.Substring(11, 4);
            if (mode == "CLEW") {
                Mode = Mode.Clew;
                edge = 200;
            }
            else if (mode == "SLEW") {
                Mode = Mode.Slew;
                edge = 400;
            }
            string freq = allLog.Substring(25, 6).Replace('.', ',');

            Freq = float.Parse(freq);

            MaxSize = signal.GetMaxInFrames();

            AverageSize = signal.GetAverageSizeInFrames();

            if (signal.SignalEntries.Count != 0)
                LastWorkingTime = signal.SignalEntries.Where(x => x.Type != EntryType.Error).Last().Time.ToString();


            activeEntries = signal.SignalEntries.Where(x => x.Type != EntryType.Error && (x.Size - x.Errors) > edge).ToList();
            if (activeEntries.Count != 0)
                LastActiveTime = activeEntries.Last().Time.ToString();
        }
        public void Copy(DirectoryInfo destination)
        {
            DirectoryInfo di = new DirectoryInfo(Directory);
            FileInfo[] files = di.GetFiles();
            if (!destination.Exists)
                destination.Create();
            DirectoryInfo dest = new DirectoryInfo(destination.ToString() + '\\' + di.Name);
            dest.Create();
            foreach (FileInfo file in files)
            {
                file.CopyTo(dest.FullName + '\\' + file.Name, true);
            }
        }

        public bool IsActive()
        {
            return IsActive(signal.SignalEntries.First().Time, signal.SignalEntries.Last().Time);
        }

        public bool IsActive(DateTime start, DateTime end) {
            if (start > end)
            {
                return IsActive(start, DateTime.Parse("23:59:59")) || IsActive(DateTime.Parse("00:00"), end);
            }
            List<SignalEntry> messages = signal.SignalEntries.Where(x =>
                x.Time >= start &&
                x.Time < end &&
                x.Type != EntryType.Error).ToList();

            int edge = 0;
            switch (Mode)
            {
                case Mode.Clew:
                    edge = 200;
                    break;
                case Mode.Slew:
                    edge = 400;
                    break;
                default:
                    break;
            }

            foreach (SignalEntry message in messages)
            {
                if (message.Size - message.Errors > edge)
                    return true;
            }
            return false;
        }

        public bool IsWorking()
        {
            return IsWorking(signal.SignalEntries.First().Time, signal.SignalEntries.Last().Time);
        }

        public bool IsWorking(DateTime start, DateTime end)
        {
            if (end < start)
            {
                return IsWorking(start, DateTime.Parse("23:59:59")) || IsWorking(DateTime.Parse("00:00"), end);
            }
            else
            {
                List<SignalEntry> entries = Signal.SignalEntries.Where(x => x.Time >= start && x.Time < end).ToList();

                if (entries.Any())
                    return true;
                else
                    return false;
            }    
        }

        #endregion
    }
}
