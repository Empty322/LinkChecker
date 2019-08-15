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
using System.Media;

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
                UpdateSizeChart();
                UpdateWorkingChart();
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
                OnPropertyChanged("CanEditCollection");
            }
        }
        public bool SynchronyzeWithVenturOn
        {
            get { return seanseManager.SynchronyzeWithVenturOn; }                
            set
            {
                seanseManager.SynchronyzeWithVenturOn = value;
                OnPropertyChanged("SynchronyzeWithVenturOn");
                OnPropertyChanged("CanEditCollection");
            }
        }
        public bool CanEditCollection { 
            get {
                return !SynchronyzeWithVenturOn;
            }
        }
        public DirectoryInfo DestPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(seanseManager.DestinationPath))
                    return new DirectoryInfo(SeanseManager.DestinationPath);
                return null;
            }
        }
        public bool DestPathSelected {
            get { return !string.IsNullOrWhiteSpace(SeanseManager.DestinationPath); } 
        }
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
        public bool NotifyWhenEndWorking { get
            {
                return notifyWhenEndWorking;
            }
            set
            {
                notifyWhenEndWorking = value;
                OnPropertyChanged("NotifyWhenEndWorking");
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
        public bool NotifyWhenEndActive
        {
            get
            {
                return notifyWhenEndActive;
            }
            set
            {
                notifyWhenEndActive = value;
                OnPropertyChanged("NotifyWhenEndActive");
            }
        }

        #region StatusBarProps

        public string Version { get { return version; } }
        public bool IsLoading {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }
        public bool IsCopying
        {
            get
            {
                return isCopying;
            }
            set
            {
                isCopying = value;
                OnPropertyChanged("IsCopying");
            }
        }
        public bool IsUpdating
        {
            get
            {
                return isUpdating;
            }
            set
            {
                isUpdating = value;
                OnPropertyChanged("IsUpdating");
            }
        }
        public int ActiveCount { get { return Seanses.Where(x => x.State == SeanseState.Active).Count(); } }
        public int WorkingCount { get { return Seanses.Where(x => x.State != SeanseState.WorkingLevel0).Count(); } }
        

        #endregion

        #endregion

        #region Fields

        private ObservableCollection<Seanse> seanses;
        private Seanse selectedSeanse;
        private SeanseManager seanseManager;
        private ILogger logger;
        private string lastSelectedPathWithLinks;
        private bool notifyWhenStartWorking;
        private bool notifyWhenEndWorking;
        private bool notifyWhenStartActive;
        private bool notifyWhenEndActive;
        private string version;
        private bool isLoading;
        private bool isCopying;
        private bool isUpdating;
        private Settings settings;
        private MainWindow window;

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
        public ICommand OpenSettings { get; set; }
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
            this.seanses = new ObservableCollection<Seanse>();
            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.notifyWhenStartActive = false;
            this.notifyWhenStartActive = false;
            this.lastSelectedPathWithLinks = "";

            seanseManager.SeanseAdding += SeanseManager_SeanseAdding;
            seanseManager.SeanseAdded += SeanseManager_SeanseAdded;
            seanseManager.SeanseRemoved += SeanseManager_SeanseRemoved;
            seanseManager.SeanseUpdated += SeanseManager_SeanseUpdated;
            seanseManager.CopyingStarted += SeanseManager_CopyingStarted;
            seanseManager.CopyingEnded += SeanseManager_CopyingEnded;
            seanseManager.UpdatingStarted += SeanseManager_UpdatingStarted;
            seanseManager.UpdatingEnded += SeanseManager_UpdatingEnded;
            
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
            tuningSeries.Color = System.Drawing.Color.Blue;
            tuningSeries.BorderWidth = 1;
            tuningChart.Series.Add(tuningSeries);


            Chart sizeChart = window.GetSizeChart();
            ChartArea sizeArea = new ChartArea("SizeArea");
            sizeChart.ChartAreas.Add(sizeArea);

            Series sizeSeries = new Series("Size");
            sizeSeries.IsXValueIndexed = true;
            sizeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            sizeSeries.XValueType = ChartValueType.Time;
            sizeSeries.XValueMember = "Time";
            sizeSeries.YValueMembers = "Size";
            sizeSeries.Color = System.Drawing.Color.Blue;
            sizeSeries.BorderWidth = 1;
            sizeChart.Series.Add(sizeSeries);


            Chart workingChart = window.GetWorkingChart();
            ChartArea workingArea = new ChartArea("WorkingArea");
            workingArea.AxisX.IntervalType = DateTimeIntervalType.Auto;
            workingArea.AxisX.Interval = settings.WorkingChartInterval;
            workingArea.AxisY.Interval = 1;
            workingChart.ChartAreas.Add(workingArea);
            
            Series workingSeries = new Series("Working");
            workingSeries.IsXValueIndexed = true;
            workingSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            workingSeries.XValueType = ChartValueType.Time;
            workingSeries.XValueMember = "Time";
            workingSeries.YValueMembers = "State";
            workingSeries.Color = System.Drawing.Color.Blue;
            workingSeries.BorderWidth = 2;
            workingChart.Series.Add(workingSeries);

            #endregion

            #region Loading seanses

            if (File.Exists("seanses.json"))
            {
                string jsonFile = "";
                List<String> dirs = new List<string>();
                try
                {
                    jsonFile = File.ReadAllText("seanses.json", Encoding.Default);
                    dirs = JsonConvert.DeserializeObject<List<string>>(jsonFile);
                }
                catch (Exception e)
                {
                    logger.LogMessage(e.Message, LogLevel.Error);
                }
                foreach (string dir in dirs)
                {
                    seanseManager.AddSeanse(dir);
                }
            }

            #endregion

            #region SettingCommands

            SelectDestinationPath = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (Directory.Exists(settings.InitialDestPath))
                    fbd.SelectedPath = settings.InitialDestPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    SeanseManager.DestinationPath = fbd.SelectedPath;
                    OnPropertyChanged("DestPath");
                    OnPropertyChanged("DestPathSelected");
                }
            });

            AddSeanse = new RelayCommand(async () =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (!string.IsNullOrWhiteSpace(lastSelectedPathWithLinks))
                    fbd.SelectedPath = lastSelectedPathWithLinks;
                else if (Directory.Exists(settings.InitialSeansesPath))
                    fbd.SelectedPath = settings.InitialSeansesPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                {
                    if (!await SeanseManager.AddSeanseAsync(fbd.SelectedPath))
                        logger.LogMessage("Не удалось добавить сеанс: \n" + fbd.SelectedPath, LogLevel.Warning);
                }
                lastSelectedPathWithLinks = fbd.SelectedPath;
            });

            AddSeansesFromVentur = new RelayCommand(async () =>
            {
                await SeanseManager.AddSeansesFromVentursFileAsync(settings.VenturFile);
            });

            AddAllSeanses = new RelayCommand(async () =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (Directory.Exists(settings.InitialDestPath))
                    fbd.SelectedPath = settings.InitialDestPath;
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                    await SeanseManager.AddAllSeansesFromFolderAsync(fbd.SelectedPath);
            });

            RemoveSeanse = new RelayCommand(() =>
            {
                try
                {
                    if (SelectedSeanse != null)
                        SeanseManager.RemoveSeanse(SelectedSeanse);
                    else
                        MessageBox.Show("Выбирете сеанс", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    if (!string.IsNullOrWhiteSpace(SeanseManager.DestinationPath)){
                        await SeanseManager.CopySeansesAsync();
                    }
                    else
                        MessageBox.Show("Папка для накопления не выбрана", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });

            UpdateSeanses = new RelayCommand(async () =>
            {
                await SeanseManager.UpdateSeansesAsync();
            });

            OpenSettings = new RelayCommand(() =>
            {
                SettingsForm sf = new SettingsForm(settings, logger);
                sf.ShowDialog();
                window.GetWorkingChart().ChartAreas[0].AxisX.Interval = settings.WorkingChartInterval;
                window.GetWorkingChart().Invalidate();
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

        private void SeanseManager_SeanseUpdated(object sender, Seanse seanse)
        {
            OnPropertyChanged("ActiveCount");
            OnPropertyChanged("WorkingCount");
            if (seanse == SelectedSeanse)
            {
                window.Dispatcher.BeginInvoke((ThreadStart)delegate()
                {
                    UpdateTuningChart();
                    UpdateWorkingChart();
                    UpdateSizeChart();
                });
            }
        }

        private void SeanseManager_SeanseRemoved(object sender, Seanse seanse)
        {
            window.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                Seanses.Remove(seanse);
            });
        }

        void SeanseManager_SeanseAdding(object sender, string directory)
        {
            IsLoading = true;
        }

        private void SeanseManager_SeanseAdded(object sender, Seanse newSeanse)
        {
            IsLoading = false;
            newSeanse.WorkingStart += Seanse_WorkingStart;
            newSeanse.ActiveStart += Seanse_ActiveStart;
            newSeanse.WorkingEnd += Seanse_WorkingEnd;
            newSeanse.ActiveEnd += newSeanse_ActiveEnd;
            window.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                Seanses.Add(newSeanse);
                SelectedSeanse = newSeanse;
            });
        }

        private void SeanseManager_CopyingStarted(object sender)
        {
            IsCopying = true;
        }

        private void SeanseManager_CopyingEnded(object sender)
        {
            IsCopying = false;
        }

        private void SeanseManager_UpdatingStarted(object sender)
        {
            IsUpdating = true;
        }

        private void SeanseManager_UpdatingEnded(object sender)
        {
            IsUpdating = false;
        }

        private void Seanse_ActiveStart(Seanse seanse, EventArgs args)
        {
            string msg = string.Format("Линк {0} {1} преходит в активный режим.", seanse.Freq, seanse.Mode);
            if (NotifyWhenStartActive)
            {
                Task.Run(() => MessageBox.Show(msg, "Переход в активный", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            logger.LogMessage(msg, LogLevel.Info);
        }

        private void newSeanse_ActiveEnd(Seanse seanse, EventArgs args)
        {
            string msg = string.Format("Линк {0} {1} вышел из активного режима " + settings.Configuration.MinutesToAwaitAfterEnd + " минут назад.", seanse.Freq, seanse.Mode);
            if (NotifyWhenEndActive)
            {
                Task.Run(() => MessageBox.Show(msg, "Выход из активного режима", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            logger.LogMessage(msg, LogLevel.Info);
        }

        private void Seanse_WorkingStart(Seanse seanse, EventArgs args)
        {
            string msg = string.Format("Линк {0} {1} начинает свою работу.", seanse.Freq, seanse.Mode);
            if (NotifyWhenStartWorking)
            {
                Task.Run(() => MessageBox.Show(msg, "Начало работы", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            logger.LogMessage(msg, LogLevel.Info);
        }

        private void Seanse_WorkingEnd(Seanse seanse, EventArgs args)
        {
            string msg = string.Format("Линк {0} {1} окончил свою работу " + settings.Configuration.MinutesToAwaitAfterEnd + " минут назад.", seanse.Freq, seanse.Mode);
            if (NotifyWhenEndWorking)
            {
                Task.Run(() => MessageBox.Show(msg, "Окончание работы", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            logger.LogMessage(msg, LogLevel.Info);
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

        private void UpdateSizeChart()
        {
            if (SelectedSeanse != null)
            {
                window.GetSizeChart().DataSource = SelectedSeanse.SizeChartUnits;
                window.GetSizeChart().Invalidate();
            }
        }

        private void UpdateWorkingChart()
        {
            if (SelectedSeanse != null)
            {
                window.GetWorkingChart().DataSource = SelectedSeanse.WorkingChartUnits;
                window.GetWorkingChart().Invalidate();
            }
        }
    }
}
