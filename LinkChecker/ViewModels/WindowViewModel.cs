using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Link11Checker.Core;
using Link11Checker.ViewModels.Base;
using System.Windows.Input;
using System.Windows.Forms;
using System.Diagnostics;
using Link11.Core;
using Link11.Core.Enums;
using Logger;
using Newtonsoft.Json;

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
            get { return updateTimerOn; }
            set
            {
                updateTimerOn = value;
                OnPropertyChanged("UpdateTimerOn");
            }
        }

        public bool CopyTimerOn
        {
            get { return copyTimerOn; }
            set
            {
                copyTimerOn = value;
                OnPropertyChanged("CopyTimerOn");
            }
        }

        public bool DestPathSelected {
            get { return destPathSelected; } 
            set {
                destPathSelected = value;
                OnPropertyChanged("DestPathSelected");
            } }

        public bool IsSeanseSelected
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

        #region TimerFields

        private bool updateTimerOn;

        private bool copyTimerOn;

        private const int updateCounterLimit = 0;

        private const int copyCounterLimit = 180;

        #endregion
                                    
        #endregion
        
        #region Commands
        public ICommand SelectDestinationPath { get; set; }
        public ICommand AddSeanse { get; set; }
        public ICommand AddAllSeanses { get; set; }
        public ICommand RemoveSeanse { get; set; }
        public ICommand CopySeanses { get; set; }
        public ICommand UpdateSeanses { get; set; }
        public ICommand LoadTuningGraph { get; set; }
        public ICommand OpenLog { get; set; }

        #endregion

        #region Ctor

        public WindowViewModel(SeanseManager sm, string version, ILogger logger)
        {
            #region Inisialization

            this.logger = logger;

            this.seanseManager = sm;

            this.seanses = sm.Seanses;

            this.version = version;

            this.notifyWhenStartActive = false;

            this.notifyWhenStartActive = false;

            if (!File.Exists("settings.json"))
                throw new FileNotFoundException();
            string settingsFile = File.ReadAllText("settings.json", Encoding.Default);
            this.settings.Configuration = JsonConvert.DeserializeObject<Configuration>(settingsFile);

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
                        seanseManager.AddSeanse(new Seanse(dir, logger));
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        MessageBox.Show("Сеанс не найден: \n" + dir, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            UpdateTimerOn = false;

            CopyTimerOn = false;

            destPathSelected = false;

            lastSelectedPathWithLinks = "";

            #endregion

            #region SetCommands

            SelectDestinationPath = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
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
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                {
                    Seanse s = new Seanse(fbd.SelectedPath + '\\', logger);
                    s.ActiveStart += OnActiveStart;
                    s.WorkingStart += OnWorkingStart;
                    SeanseManager.AddSeanse(s);
                }
                lastSelectedPathWithLinks = fbd.SelectedPath;
            });

            AddAllSeanses = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                    throw new NotImplementedException();
            });

            RemoveSeanse = new RelayCommand(() =>
            {
                if (SelectedSeanse != null)
                    SeanseManager.RemoveSeanse(SelectedSeanse);
                else
                    System.Windows.MessageBox.Show("Выбирете сеанс", "Ошибка");
            });

            CopySeanses = new RelayCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(SeanseManager.DestinationPath))
                    SeanseManager.CopySeanses();
            });

            UpdateSeanses = new RelayCommand(() =>
            {
                SeanseManager.UpdateSeanses();
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

            #region SetTimer

            Thread updateWorker = new Thread(() =>
            {
                int updateCounter = 0;
                int copyCounter = 0;
                try
                {
                    while (true)
                    {
                        if (UpdateTimerOn && updateCounter >= updateCounterLimit)
                        {
                            SeanseManager.UpdateSeanses();

                            updateCounter = 0;
                        }
                        if (CopyTimerOn && copyCounter >= copyCounterLimit)
                        {
                            if (!string.IsNullOrWhiteSpace(SeanseManager.DestinationPath))
                                SeanseManager.CopySeanses();
                            copyCounter = 0;
                        }
                        updateCounter++;
                        copyCounter++;
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString() + " " + e.Message);
                }
            });
            updateWorker.Start();
            updateWorker.IsBackground = true;    
            
            #endregion
        }

        #endregion

        #region EventHandlers

        private void OnActiveStart(object sender, EventArgs args)
        {
            if (NotifyWhenStartActive)
            {
                Seanse seanse = (Seanse)sender;
                MessageBox.Show(string.Format("Линк {0} {1} преходит в активный режим.", seanse.Freq, seanse.Mode), "Переход в активный");
            }
        }

        private void OnWorkingStart(object sender, EventArgs args)
        {
            if (NotifyWhenStartWorking)
            {
                Seanse seanse = (Seanse)sender;
                MessageBox.Show(string.Format("Линк {0} {1} начинает свою работу.", seanse.Freq, seanse.Mode), "Начало работы");
            }
        }

        #endregion
    }
}
