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
    public class Seanse : INotifyPropertyChanged, ICloneable
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
        public SeanseState State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                OnPropertyChanged("Visible");
            }
        }
        public int MaxSize { 
            get {
                return maxSize;
            }
            set
            {
                maxSize = value;
                OnPropertyChanged("MaxSize");
            }
        }
        public float MaxSizeInBytes { 
            get { 
                return maxSizeInBytes; 
            }
            set
            {
                maxSizeInBytes = value;
                OnPropertyChanged("MaxSizeInBytes");
            }
        }
        public float AverageSize { 
            get { 
                return averageSize; 
            }
            set {
                averageSize = value;
                OnPropertyChanged("AverageSize");
            }
        }
        public float AverageSizeInBytes { 
            get { 
                return averageSizeInBytes;
            }
            set {
                averageSizeInBytes = value;
                OnPropertyChanged("AverageSizeInBytes");
            }
        }
        public int AbonentsCount
        {
            get
            {
                return abonentsCount;
            }
            set
            {
                abonentsCount = value;
                OnPropertyChanged("AbonentsCount");
            }
        }
        public string Intervals
        {
            get
            {
                return intervals;
            }
            set
            {
                intervals = value;
                OnPropertyChanged("Intervals");
            }
        }
        public int PercentReceiving
        {
            get
            {
                return percentReceiving;
            }
            set 
            {
                percentReceiving = value;
                OnPropertyChanged("Percentreceiving");
            }                
        }
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
                OnPropertyChanged("Remark");
            }
        }
        public DateTime StartWorkingTime
        {
            get
            {
                return startWorkingTime;
            }
            set
            {
                startWorkingTime = value;
                OnPropertyChanged("StartWorkingTime");
            }
        }
        public DateTime LastWorkingTime
        {
            get
            {
                return lastWorkingTime;
            }
            set
            {
                lastWorkingTime = value;
                OnPropertyChanged("LastWorkingTime");
            }
        }
        public DateTime LastCopy
        {
            get
            {
                return lastCopy;
            }
            set
            {
                lastCopy = value;
                OnPropertyChanged("LastCopy");
            }
        }
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                lastUpdate = value;
                OnPropertyChanged("LastUpdate");
            }
        }
        public List<TuningChartUnit> TuningChartUnits { get; set; }
        public List<WorkingChartUnit> WorkingChartUnits { get; set; }
        public List<SizeChartUnit> SizeChartUnits { get; set; }
        public List<ActiveEntry> ActiveEntries
        {
            get
            {
                return activeEntries;
            }
            set
            {
                activeEntries = value;
                OnPropertyChanged("ActiveEntries");
            }
        }
        public List<AbonentInfo> Abonents { get; set; }

        #endregion

        #region Fields

        private List<SignalEntry> signalEntries;
        private bool directoryExists;
        private float freq;
        private Mode mode;
        private SeanseState state;
        private bool visible;
        private string remark;
        private DateTime startWorkingTime;
        private DateTime lastWorkingTime;
        private DateTime lastCopy;
        private DateTime lastUpdate;
        private int maxSize;
        private float maxSizeInBytes;
        private float averageSize;
        private float averageSizeInBytes;
        private int abonentsCount;
        private string intervals;
        private int percentReceiving;
        private List<ActiveEntry> activeEntries;

        private IParser parser;
        private ILogger logger;   
        private Configuration config;
        private SeanseState prevState;
        private bool isEnded;
        private bool isActiveEnded;
        private long lastUpdateLogFileLength;
        private long lastCopyLogFileLenght;
        private DateTime lastModified;
        private DateTime serverTime;

        private const int countForWorkingLevel5 = 200;
        private const int countForWorkingLevel4 = 100;
        private const int countForWorkingLevel3 = 40;
        private const int countForWorkingLevel2 = 10;
        private const int countForWorkingLevel1 = 1;
                
        #endregion

        #region Ctor

        public Seanse(string directory, Configuration config) : this(directory, config, new Parser(), new PrimitiveLogger("log.txt", LogLevel.Error)) { }

        public Seanse(string directory, Configuration config, IParser parser, ILogger logger) 
        {
            this.Directory = new DirectoryInfo(directory);
            this.DirectoryExists = Directory.Exists;
            if (!DirectoryExists)
                throw new LogFileNotFoundException();
            this.TuningChartUnits = new List<TuningChartUnit>();
            this.WorkingChartUnits = new List<WorkingChartUnit>();
            this.Abonents = new List<AbonentInfo>();
            this.activeEntries = new List<ActiveEntry>();
            this.lastModified = new DateTime();
            this.remark = "";
            this.logger = logger;
            this.signalEntries = new List<SignalEntry>();
            this.parser = parser;
            this.config = config;
            this.mode = Mode.Unknown;
            this.prevState = SeanseState.WorkingLevel0;
            this.state = SeanseState.WorkingLevel0;
            this.isEnded = true;
            this.isActiveEnded = true;
            this.lastCopyLogFileLenght = -1;
            this.lastUpdateLogFileLength = -1;
            this.serverTime = new DateTime();
            Update();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            StringBuilder messageToLog = new StringBuilder();
            messageToLog.Append("ОБНОВЛЕНИЕ СЕАНСА " + Directory + Environment.NewLine);
            // Если директория существует
            if (!System.IO.Directory.Exists(Directory.FullName))
            {
                DirectoryExists = false;
                return;
            }
            messageToLog.Append(" + Директория существует" + Environment.NewLine);

            DirectoryExists = true;

            // Размер лог файла
            long logFileLength = new FileInfo(Directory + "\\log.txt").Length;
            if (logFileLength != lastUpdateLogFileLength)
            {
                messageToLog.Append(" + logFileLength != lastUpdateLogFileLenght (" + logFileLength + " != " + lastUpdateLogFileLength + ")" + Environment.NewLine);

                // Загрузить log.txt
                LoadAllLog();
                messageToLog.Append(" + Загрузка 'alllog.txt'" + Environment.NewLine);

                // Загрузить allLog.txt
                LoadLog();
                messageToLog.Append(" + Загрузка 'log.txt'" + Environment.NewLine);

                // Если это не пустой сеанс
                if (signalEntries.Any())
                {
                    messageToLog.Append(" + Сеананс не пустой " + signalEntries.Count + Environment.NewLine);

                    // Обновить время сервера
                    DateTime lastEntryTime = signalEntries.Last().Time;
                    TimeSpan delay = lastModified - lastEntryTime;
                    serverTime = DateTime.Now - delay;      

                    // Получить данные для графика расстройки
                    TuningChartUnits = GetTuningChartUnits(config.SmoothValue);

                    // Получить данные для графика объема
                    SizeChartUnits = GetSizeChartUnits();

                    // Получить данные для графика работы
                    WorkingChartUnits = GetWorkingChartUnits(new TimeSpan(0, 1, 0));

                    messageToLog.Append(" + Единицы графиков вычислины " + TuningChartUnits.Count + " " + SizeChartUnits.Count + " " + WorkingChartUnits.Count + Environment.NewLine);

                    // Получить вхождения с объемом, превышающим норму
                    List<ActiveEntry> newActiveEntries = signalEntries
                        .Where(x =>
                            x.Type != EntryType.Error &&
                            ((x.Size - x.Errors) > (int)Mode))
                        .Select(x =>
                            new ActiveEntry { Time = x.Time, Size = x.Size - x.Errors })
                        .ToList();
                    ActiveEntries = newActiveEntries;
                    messageToLog.Append(" + Активные сообщения получены " + ActiveEntries.Count + Environment.NewLine);

                    // Получить абонентов
                    Abonents = GetAbonentsInfo();
                    AbonentsCount = GetActualAbonentsCount(Abonents);
                    messageToLog.Append(" + Абоненты получены " + Abonents.Count + Environment.NewLine);

                    // Установить время начала
                    StartWorkingTime = signalEntries.First().Time;

                    // Обновить конечное время
                    for (int index = signalEntries.Count - 1; index >= 0; index--)
                        if (signalEntries[index].Type != EntryType.Error)
                        {
                            lastWorkingTime = signalEntries[index].Time;
                            break;
                        }

                    // Обновить сторку с интервалами
                    string intervals = "";
                    var intervalEntries = GetIntervals().OrderByDescending(interval => interval.Value).Take(3);
                    foreach (var interval in intervalEntries)
                        intervals += interval.Key + "(" + interval.Value + ") ";
                    this.intervals = intervals; 

                    // Обновить процент приема
                    int notErrorsCount = signalEntries.Where(x => x.Type != EntryType.Error).Count();
                    if (notErrorsCount > 0)
                        PercentReceiving = (int)Math.Round(100f / signalEntries.Count() * notErrorsCount);
                    else
                        PercentReceiving = 0;
                        
                    // Обновить переменные объема
                    MaxSize = GetMaxInFrames();
                    MaxSizeInBytes = (float)Math.Round(MaxSize * 3.75f, 2);
                    AverageSize = (float)Math.Round(GetAverageSizeInFrames());
                    AverageSizeInBytes = (float)Math.Round(AverageSize * 3.75f, 2);

                    // Обновить время последнего изменения
                    lastModified = File.GetLastWriteTime(Directory + "\\log.txt");
                    messageToLog.Append(" + lastModified = " + lastModified + Environment.NewLine);
                    // Обновить время последнего обновления
                    LastUpdate = DateTime.Now;
                    messageToLog.Append(" + lastUpdate = " + lastUpdate + Environment.NewLine);
                    // Обновить размер лога при последнем обновлении
                    lastUpdateLogFileLength = logFileLength;
                    messageToLog.Append(" + lastUpdateLogFileLength = " + lastUpdateLogFileLength + Environment.NewLine);
                }
            }

            // Обновить видимость
            if (config.HideEmptySeanses && !(signalEntries.Where(e => e.Type != EntryType.Error).Count() > config.Trashold))
                Visible = false;
            else
                Visible = true; 

            // Узнать состояние сеанса
            State = GetState();
            messageToLog.Append(" + prevState = " + prevState + Environment.NewLine);
            messageToLog.Append(" + State = " + State + Environment.NewLine);

            logger.LogMessage(messageToLog.ToString(), LogLevel.Info);

            // Запустить уведомления
            FireEvents(prevState, state);

            prevState = state; 
        }

        public bool Copy(DirectoryInfo destination)
        {
            bool result = false;

            // Если директория не существует выйти
            if (!System.IO.Directory.Exists(Directory.FullName))
            {
                DirectoryExists = false;
                return result;
            }

            // Если нет вхождений выйти
            if (!signalEntries.Any())
            {
                return result;
            }

            FileInfo logInfo = new FileInfo(Directory + "/log.txt");
            // Его размер не равен размеру при прошлом копировании
            // Если размер лога больше порога размера и
            // Превышает порог приема и
            bool seanseNeedsToUpdate = logInfo.Length != lastCopyLogFileLenght &&
                                        logInfo.Length >= config.CopyLengthTrashold &&
                                        PercentReceiving >= config.CopyPercentTrashold;

            // Если сигнал не работает и он нуждается в копировании
            if (!seanseNeedsToUpdate)
            {
                return result;
            }

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
                        // Обновить время последнего копирования
                        LastCopy = DateTime.Now;
                        // Обновить размер лог файла при последнем копировании
                        lastCopyLogFileLenght = logInfo.Length;
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            }
            return result;
        }

        public void Delete()
        {
            try
            {
                FileInfo[] log = Directory.GetFiles("log.txt");
                if (log.Count() != 0)
                    log[0].Delete();
                Directory.Delete(true);
                DirectoryExists = false;
            }
            catch
            {
                logger.LogMessage("Невозможно удалить папку " + Directory, LogLevel.Warning);
            }
        }

        public int GetActualAbonentsCount(List<AbonentInfo> abonents)
        {
            int resCount = 0;
            if (abonents.Count() > 0)
            {
                int max = abonents.Max(x => x.Count);
                int i = 0;
                while (i < abonents.Count)
                {
                    if (Abonents[i].Count > max * config.AbonentsK)
                        resCount++;
                    i++;
                }
            }
            return resCount;
        }

        public List<int> GetAbonents()
        {
            List<int> abonents = new List<int>();
            if (signalEntries.Count != 0)
                abonents = GetAbonents(signalEntries.First().Time, signalEntries.Last().Time, 0);
            return abonents;
        }

        public List<int> GetAbonents(DateTime start, DateTime end, double abonentsK)
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
                if (abonentsEntries.Count() > 0)
                {
                    int max = abonentsEntries.Max(x => x.Value);
                    foreach (var abonent in abonentsEntries)
                    {
                        if (abonent.Value < max * abonentsK)
                        {
                            abonents.Remove(abonent.Key);
                        }
                    }
                }
            }
            return abonents;
        }

        public List<int> GetAbonentsWithInterval(int interval, float intervalsK, double abonentsK)
        {
            List<int> abonetns = new List<int>();
            abonetns = GetAbonentsWithInterval(signalEntries.First().Time, signalEntries.Last().Time, interval, intervalsK, abonentsK);
            return abonetns;
        }

        public List<int> GetAbonentsWithInterval(DateTime start, DateTime end, int interval, double intervalsK, double abonentsK)
        {
            List<int> abonents = GetAbonents(start, end, abonentsK);
            return GetAbonentsWithInterval(start, end, abonents, interval, intervalsK);
        }

        public List<int> GetAbonentsWithInterval(DateTime start, DateTime end, List<int> abonents, int interval, double intervalsK)
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
                    if (count > intervals.Count() * intervalsK)
                        result.Add(abonent);
                }
            }
            return result;
        }

        public Dictionary<int, int> GetIntervals(params int[] abonents)
        {
            Dictionary<int, int> intervals = new Dictionary<int, int>();
            if (signalEntries.Any())
                intervals = GetIntervals(signalEntries.First().Time, signalEntries.Last().Time, abonents);
            return intervals;
        }

        public Dictionary<int, int> GetIntervals(DateTime start, DateTime end, params int[] abonents)
        {
            Dictionary<int, int> intervalEntries = new Dictionary<int, int>();
            if (signalEntries.Count != 0)
            {
                List<SignalEntry> currentEntries;
                if (abonents.Any())
                    currentEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Ninterval != 0 && (x.Abonent.HasValue ? abonents.Contains(x.Abonent.Value) : false)).ToList();
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

        public int GetMaxInFrames()
        {
            int maxInFrames = 0;
            if (signalEntries.Count != 0)
                maxInFrames = GetMaxInFrames(signalEntries.First().Time, signalEntries.Last().Time);
            return maxInFrames;
        }

        public int GetMaxInFrames(DateTime start, DateTime end)
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

        public float GetAverageSizeInFrames()
        {
            float avgInFrames = 0;
            if (signalEntries.Count != 0)
                avgInFrames = GetAverageSizeInFrames(signalEntries.First().Time, signalEntries.Last().Time);
            return avgInFrames;
        }

        public float GetAverageSizeInFrames(DateTime start, DateTime end)
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

        public void SetConfuguration(Configuration config)
        {
            // Если усреднение графика расстройки изменилось, то перерасчитать значения
            if (this.config.SmoothValue != config.SmoothValue)
                TuningChartUnits = GetTuningChartUnits(config.SmoothValue);

            // Установить конфигурацию
            this.config = config;
        }
        
        public object Clone()
        {
            return this.MemberwiseClone();
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
                // Попробовать парсить allLog.txt
                parser.ParseAllLog(allLogFileContent, out freq, out mode);
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
                catch (Exception e)
                {
                    logger.LogMessage(e.Message, LogLevel.Warning);
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

        private List<AbonentInfo> GetAbonentsInfo()
        {
            List<AbonentInfo> abonetnsInfo = new List<AbonentInfo>();
            if (signalEntries.Any())
                abonetnsInfo = GetAbonentsInfo(signalEntries.First().Time, signalEntries.Last().Time);
            return abonetnsInfo;
        }

        private List<AbonentInfo> GetAbonentsInfo(DateTime start, DateTime end)
        {
            List<AbonentInfo> result = new List<AbonentInfo>();
            List<int> abonents = GetAbonents();
            foreach (int abonent in abonents)
            {
                Dictionary<int, int> abonentIntervals = GetIntervals(start, end, abonent);
                AbonentInfo info = new AbonentInfo(abonent);
                info.Count = signalEntries.Where(e => e.Abonent.HasValue && e.Abonent.Value == abonent).Count();
                info.UpdateIntervals(abonentIntervals);
                result.Add(info);
            }
            return result.OrderByDescending(x => x.Count).ToList();
        }

        private SeanseState GetState() {
            SeanseState result = SeanseState.WorkingLevel0;

            if (signalEntries.Count != 0)
            {
                List<SignalEntry> lastEntries = signalEntries.Where(x => x.Time > serverTime - new TimeSpan(0, 3, 0) && x.Time < serverTime).ToList();
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

        private void FireEvents(SeanseState prevState, SeanseState state)
        {
            // Eсли перешел в активный
            if (isActiveEnded && prevState != SeanseState.Active && state == SeanseState.Active)
            {
                isActiveEnded = false;
                ActiveStart.Invoke(this, new EventArgs());
            }

            // Если Вышел из активного
            IEnumerable<SignalEntry> activeEntries = signalEntries.Where(x => x.Type != EntryType.Error && x.Size - x.Errors > (int)Mode);
            if (!isActiveEnded && activeEntries.Any() && !(activeEntries.Last().Time > serverTime.AddMinutes(-config.MinutesToAwaitAfterEnd) && activeEntries.Last().Time <= serverTime))
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
            if (!isEnded && signalEntries.Where(x => x.Time > serverTime.AddMinutes(-config.MinutesToAwaitAfterEnd) && x.Time <= serverTime).Count() == 0)
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

        #region LogFileNotFoundException

        public class LogFileNotFoundException : FileNotFoundException
        {
            public LogFileNotFoundException() { }
        }

        #endregion
  
    }
}
