using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11Core;
using System.IO;
using Link11Checker.ViewModels.Base;
using Logger;

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

        public string LastWorkingTime
        {
            get {
                if (Signal.SignalEntries.Count == 0)
                    return "";
                return Signal.SignalEntries.Where(x => x.Type != EntryType.Error).Last().Time.ToShortTimeString();
            }
        }
        public string LastActiveTime
        {
            get { 
                IEnumerable<SignalEntry> active = Signal.SignalEntries.Where(x => x.Type != EntryType.Error && ((x.Size > 200 && Mode == Mode.Clew) ||  (x.Size > 400 && Mode == Mode.Slew)));
                if (active.Count() == 0)
                    return "";
                return active.Last().Time.ToShortTimeString();
            }
        }

        public int AbonentsCount {
            get { return Signal.GetAbonents().Count; }
        }

        public string Intervals {
            get { return Signal.GetIntervals(); }
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
            get { return Signal.GetMaxInFrames(); }
        }

        public float AverageSize
        {
            get { return (float)Math.Round(Signal.GetAverageSizeInFrames(), 1); }
        }

        #endregion

        #region fields

        private readonly Signal signal;
        private float freq;
        private Mode mode;
        private string position;
        private string coordinates;
        private int edge;
        private ILogger logger;

        #endregion

        #region Ctor

        public Seanse(string directory, ILogger logger) 
        {
            this.Directory = directory;
            this.signal = new Signal(new Parser(), new Configuration { AbonentsK = 0.1, IntervalsK = 0.05 });
            this.edge = 0;
            this.logger = logger;
            Update();
        }

        #endregion

        #region Methods

        public void Update()
        {
            logger.LogMessage(" UPDATING " + Directory, LogLevel.Info);

            signal.LoadSignal(Directory + "log.txt");

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
                OnPropertyChanged("LastWorkingTime");
                OnPropertyChanged("LastActiveTime");
                OnPropertyChanged("LastWorkingTime");
                OnPropertyChanged("AbonentsCount");
                OnPropertyChanged("Intervals");
                OnPropertyChanged("MaxSize");
                OnPropertyChanged("AverageSize");
                
            }
            catch (Exception e)
            {
                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
            }
        }
        public void Copy(DirectoryInfo destination)
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
                    logger.LogMessage("     COPYING FILE" + file.FullName, LogLevel.Info);
                }  
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            }
            
        }

        public bool IsActive()
        {
            if (signal.SignalEntries.Count == 0)
                return false;
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
            if (signal.SignalEntries.Count == 0)
                return false;
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
