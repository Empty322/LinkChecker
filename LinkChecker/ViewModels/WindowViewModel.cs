using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using Link11Checker.Core;
using Link11Checker.ViewModels.Base;
using System.Windows.Input;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using Link11.Core;
using Link11.Core.Enums;
using Logger;
using Newtonsoft.Json;
using Link11.Core.Charting;
using System.Windows.Forms;

namespace Link11Checker.ViewModels
{
    public class WindowViewModel : BaseViewModel
    {
        
        #region Properties

        public ObservableCollection<Seanse> Seanses { 
            get { return seanses; } 
            private set {
                seanses = value;
                OnPropertyChanged("Seanses");
            } }
        public Seanse SelectedSeanse { 
            get { return selectedSeanse; } 
            set {
                selectedSeanse = value;
                UpdateTuningChart();
                OnPropertyChanged("IsSeanceSelected");
                OnPropertyChanged("SelectedSeanse");
            } } 
        public SeanseManager SeanseManager { 
            get { return seanseManager; }
            set {
                seanseManager = value;
                OnPropertyChanged("SeanseManager");
            } }
        public bool UpdateTimerOn {
            get { return seanseManager.UpdateTimerOn; }
            set
            {
                seanseManager.UpdateTimerOn = value;
                OnPropertyChanged("UpdateTimerOn");
            }
        }
        public bool CopyTimerOn
        {
            get { return seanseManager.CopyTimerOn; }
            set
            {
                seanseManager.CopyTimerOn = value;
                OnPropertyChanged("CopyTimerOn");
            }
        }
        public bool DestPathSelected {
            get { return destPathSelected; } 
            set {
                destPathSelected = value;
                OnPropertyChanged("DestPathSelected");
            } }
        public bool IsSeanceSelected
        {
            get { return selectedSeanse == null ? false : true; }
        }
        public bool NotifyWhenStartWorking
        {
            get
            {
                return notifyWhenStartWorking;
            }
            set
            {
                notifyWhenStartWorking = value;
                OnPropertyChanged("NotifyWhenStartWorking");
            }
        }
        public bool NotifyWhenStartActive
        {
            get
            {
                return notifyWhenStartActive;
            }
            set
            {
                notifyWhenStartActive = value;
                OnPropertyChanged("NotifyWhenStartActive");
            }
        }
        public int ChartMax
        {
            get
            {
                return chartMax;
            }
            set
            {
                chartMax = value;
                window.GetTuningChart().ChartAreas[0].AxisY.Maximum = chartMax;
                window.GetTuningChart().ChartAreas[0].AxisY.Minimum = -chartMax;
                OnPropertyChanged("ChartMax");
            }
        }


        #region StatusBarProps

        public string Version { get { return version; } }
        public int ActiveCount { get { return Seanses.Where(x => x.State == SeanseState.Active).Count(); } }
        public int WorkingCount { get { return Seanses.Where(x => x.State != SeanseState.WorkingLevel0).Count(); } }

        #endregion

        #endregion

        #region Fields

        private ObservableCollection<Seanse> seanses;
        private Seanse selectedSeanse;
        private SeanseManager seanseManager;
        private ILogger logger;
        private bool destPathSelected;
        private string lastSelectedPathWithLinks;
        private bool notifyWhenStartWorking;
        private bool notifyWhenStartActive;
        private string version;
        private Settings settings;
        private MainWindow window;
        private int chartMax;

        #endregion

        #region Commands

        public ICommand SelectDestinationPath { get; set; }
        public ICommand AddSeanse { get; set; }
        public ICommand AddSeansesFromVentur { get; set; }
        public ICommand AddAllSeanses { get; set; }
        public ICommand RemoveSeanse { get; set; }
        public ICommand RemoveAllSeanses { get; set; }
        public ICommand CopySeanses { get; set; }
        public ICommand UpdateSeanses { get; set; }
        public ICommand Settings { get; set; }
        public ICommand About { get; set; }
        public ICommand OpenLog { get; set; }

        #endregion

        #region Ctor

        public WindowViewModel(MainWindow wnd ,SeanseManager sm, Settings settings, ILogger logger)
        {
            this.logger = logger;
            this.settings = settings;
            this.window = wnd;
            this.seanseManager = sm;
            this.seanses = sm.Seanses;
            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.notifyWhenStartActive = false;
            this.notifyWhenStartActive = false;
            this.destPathSelected = false;
            this.lastSelectedPathWithLinks = "";

            seanseManager.SeanseLoaded += SeanseManager_SeanseLoaded;
            seanseManager.SeansesUpdated += seanseManager_SeanseUpdated;
            seanseManager.Seanses.CollectionChanged += Seanses_CollectionChanged;
            
            
            #region Charts initialization

            Chart tuningChart = window.GetTuningChart();
            ChartArea tuningArea = new ChartArea("TuningArea");
            tuningChart.ChartAreas.Add(tuningArea);


            Series tuningSeries = new Series("Tuning");
            tuningSeries.IsXValueIndexed = true;
            tuningSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            tuningSeries.XValueType = ChartValueType.Time;
            tuningSeries.XValueMember = "Time";
            tuningSeries.YValueMembers = "Tuning";
            tuningSeries.Color = System.Drawing.Color.Black;
            tuningSeries.BorderWidth = 1;
            tuningChart.Series.Add(tuningSeries);

            #endregion

            #region Loading seanses

            if (File.Exists("seanses.json"))
            {
                string jsonFile = "";
                try
                {
                    jsonFile = File.ReadAllText("seanses.json", Encoding.Default);
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.Message, LogLevel.Error);
                }
                List<String> dirs = JsonConvert.DeserializeObject< List<string> >(jsonFile);
                foreach (string dir in dirs)
                {
                    try
                    {
                        Seanse s = new Seanse(dir, logger);
                        s.ActiveStart += OnActiveStart;
                        s.WorkingStart += OnWorkingStart;
                        seanseManager.AddSeanse(s);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        MessageBox.Show("Сеанс не найден: \n" + dir, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Seanse.LogFileNotFoundException e)
                    {
                        logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                    }
                }
            }

            #endregion

            #region SetCommands

            SelectDestinationPath = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (Directory.Exists(settings.InitialDestPath))
                    fbd.SelectedPath = settings.InitialDestPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    SeanseManager.DestinationPath = fbd.SelectedPath;
                    DestPathSelected = true;
                }
            });

            AddSeanse = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (!string.IsNullOrWhiteSpace(lastSelectedPathWithLinks))
                    fbd.SelectedPath = lastSelectedPathWithLinks;
                else if (Directory.Exists(settings.InitialSeansesPath))
                    fbd.SelectedPath = settings.InitialSeansesPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                {
                    try
                    {
                        Seanse s = new Seanse(fbd.SelectedPath + '\\', logger);
                        s.ActiveStart += OnActiveStart;
                        s.WorkingStart += OnWorkingStart;
                        SeanseManager.AddSeanse(s);
                    }
                    catch (Seanse.LogFileNotFoundException e)
                    {
                        logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                    }
                }
                lastSelectedPathWithLinks = fbd.SelectedPath;
            });

            AddSeansesFromVentur = new RelayCommand(async () =>
            {
                try
                {
                    await SeanseManager.GetSeansesFromVentursFileAsync(settings.VenturFile);
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show("Папка с сеансом не найдена: \n" + e.Data["dir"].ToString(), "Ошибка при добавлении сеансов из файла вентура");
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            });

            AddAllSeanses = new RelayCommand(async () =>
            {
                Thread t = Thread.CurrentThread;

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (Directory.Exists(settings.InitialDestPath))
                    fbd.SelectedPath = settings.InitialDestPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                    await SeanseManager.GetAllSeansesFromFolderAsync(fbd.SelectedPath);
            });

            RemoveSeanse = new RelayCommand(() =>
            {
                try
                {
                    if (SelectedSeanse != null)
                        SeanseManager.RemoveSeanse(SelectedSeanse);
                    else
                        System.Windows.MessageBox.Show("Выбирете сеанс", "Ошибка");
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            });

            RemoveAllSeanses = new RelayCommand(() =>
            {
                try {
                    DialogResult result = MessageBox.Show("Удалить все сеансы?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        SeanseManager.RemoveAllSeanses();                                 
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            });

            CopySeanses = new RelayCommand(async () =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(SeanseManager.DestinationPath))
                        await SeanseManager.CopySeansesAsync();
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show("Папка с сеансом не найдена: \n" + e.Data["dir"].ToString(), "Ошибка при копировании");
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
            });

            UpdateSeanses = new RelayCommand(async () =>
            {
                await SeanseManager.UpdateSeansesAsync();
            });

            About = new RelayCommand(() => {
                AboutBox about = new AboutBox();
                about.ShowDialog();
            });

            OpenLog = new RelayCommand(() =>
            {
                if (SelectedSeanse != null)
                {
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo("excel.exe", "\"" + SelectedSeanse.Directory + "\\log.txt\"");
                    p.Start();
                }
            });

            #endregion
        }

        

        #endregion

        #region EventHandlers
        
        private void Seanses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ActiveCount");
            OnPropertyChanged("WorkingCount");
        }
        private void seanseManager_SeanseUpdated(object sender, Seanse seanse)
        {
            OnPropertyChanged("ActiveCount");
            OnPropertyChanged("WorkingCount");
        }

        private void OnActiveStart(object sender, EventArgs args)
        {
            if (NotifyWhenStartActive)
            {
                Seanse seanse = (Seanse)sender;
                MessageBox.Show(string.Format("Линк {0} {1} преходит в активный режим.", seanse.Freq, seanse.Mode), "Переход в активный", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnWorkingStart(object sender, EventArgs args)
        {
            if (NotifyWhenStartWorking)
            {
                Seanse seanse = (Seanse)sender;
                MessageBox.Show(string.Format("Линк {0} {1} начинает свою работу.", seanse.Freq, seanse.Mode), "Начало работы", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        #endregion
                  
        private void UpdateTuningChart()
        {
            if (SelectedSeanse != null)
            {
                window.GetTuningChart().DataSource = SelectedSeanse.TuningChartUnits;
                window.GetTuningChart().Invalidate();
            }
        }
            
        private void SeanseManager_SeanseLoaded(object sender, Seanse newSeanse)
        {
            window.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                newSeanse.WorkingStart += OnWorkingStart;
                newSeanse.ActiveStart += OnActiveStart; 
                SeanseManager.AddSeanse(newSeanse);
            });
        }
    }
}
