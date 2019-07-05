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
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using Link11.Core;
using Logger;

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
                OnPropertyChanged("SelectedSeanse");
            } }

        public SeanseManager SeanseManager { 
            get { return seanseManager; }
            set {
                seanseManager = value;
                OnPropertyChanged("SeanseManager");
            } }

        public string SeanseToAdd { 
            get { return seanseToAdd; } 
            set {
                seanseToAdd = value;
                OnPropertyChanged("SeanseToAdd"); 
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

        #endregion

        #region Fields

        private ObservableCollection<Seanse> seanses;

        private Seanse selectedSeanse;

        private SeanseManager seanseManager;

        private ILogger logger;

        private string seanseToAdd;

        private bool updateTimerOn;

        private bool copyTimerOn;

        private bool destPathSelected;

        private string lastSelectedPathWithLinks;

        private const int updateCounterLimit = 0;

        private const int copyCounterLimit = 180;

        #endregion
        
        #region Commands
        public ICommand SelectDestinationPath { get; set; }
        public ICommand AddSeanse { get; set; }
        public ICommand RemoveSeanse { get; set; }
        public ICommand CopySeanses { get; set; }
        public ICommand UpdateSeanses { get; set; }
        public ICommand SetUpdateTimer { get; set; }
        public ICommand SetCopyTimer { get; set; }
        public ICommand LoadTuningGraph { get; set; }
        public ICommand OpenLog { get; set; }

        #endregion

        #region Ctor

        public WindowViewModel(SeanseManager sm, ILogger logger)
        {
            
            this.logger = logger;

            SeanseManager = sm;

            Seanses = sm.Seanses;

            if (File.Exists("seanses.txt"))
            {
                string[] dirs = File.ReadAllLines("seanses.txt");
                foreach (string dir in dirs)
                    seanseManager.AddSeanse(new Seanse(dir, logger));
            }

            UpdateTimerOn = false;

            CopyTimerOn = false;

            destPathSelected = false;

            lastSelectedPathWithLinks = "";

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
                    SeanseManager.AddSeanse(s);
                }
                lastSelectedPathWithLinks = fbd.SelectedPath;
            });

            RemoveSeanse = new RelayCommand(() =>
            {
                if (SelectedSeanse != null)
                {
                    SeanseManager.RemoveSeanse(SelectedSeanse);
                }
                else
                {
                    System.Windows.MessageBox.Show("Выбирете сеанс", "Ошибка");
                }
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
                    p.StartInfo = new ProcessStartInfo("notepad.exe", SelectedSeanse.Directory + "\\log.txt");
                    p.Start();
                }
            });

            Thread updateWorker = new Thread(() =>
            {
                int updateCounter = 0;
                int copyCounter = 0;
                while (true)
                {
                    if (UpdateTimerOn && updateCounter >= updateCounterLimit)
                    {
                        SeanseManager.UpdateSeanses();
                        updateCounter = 0;
                    }
                    if (CopyTimerOn && copyCounter >= copyCounterLimit)
                    {
                        if (string.IsNullOrWhiteSpace(SeanseManager.DestinationPath))
                            SeanseManager.CopySeanses();
                        copyCounter = 0;
                    }
                    updateCounter++;
                    copyCounter++;
                    Thread.Sleep(5000);
                }
            });
            updateWorker.Start();
            updateWorker.IsBackground = true;
        }

        #endregion

        #region Methods



        #endregion
    }
}
