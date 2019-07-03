﻿using System;
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

        #endregion

        #region Ctor

        public WindowViewModel()
        {              
            Seanses = new ObservableCollection<Seanse>();

            SeanseManager = new SeanseManager();

            Seanse test = new Seanse(@"d:\Bespalov\logs\4548.7__10_50_12\");
            Seanses.Add(test);
            SeanseManager.AddSeanse(test);   

            UpdateTimerOn = false;

            CopyTimerOn = false;

            destPathSelected = false;

            lastSelectedPathWithLinks = "";

            SelectDestinationPath = new RelayCommand(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && fbd.SelectedPath != null)
                {
                    SeanseManager.DestinationPath = fbd.SelectedPath;
                    DestPathSelected = true;
                }
            });

            AddSeanse = new RelayCommand(() =>
            {
                try
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (lastSelectedPathWithLinks != null)
                        fbd.SelectedPath = lastSelectedPathWithLinks;
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && fbd.SelectedPath != null)
                    {
                        Seanse s = new Seanse(fbd.SelectedPath + '\\');
                        Seanses.Add(s);
                        SeanseManager.AddSeanse(s);
                    }
                    lastSelectedPathWithLinks = fbd.SelectedPath;
                }
                catch (Exception e) {
                    System.Windows.MessageBox.Show(e.Message, "Ошибка при добавлении сеанса.");
                }
            });

            RemoveSeanse = new RelayCommand(() =>
            {
                if (SelectedSeanse != null)
                {
                    Seanses.Remove(SelectedSeanse);
                    SeanseManager.Seanses.Remove(SelectedSeanse);
                }
                else
                {
                    System.Windows.MessageBox.Show("Выберите сеанс для удаления.", "Ошибка");
                }
            });

            CopySeanses = new RelayCommand(() =>
            {
                try
                {
                    if (SeanseManager.DestinationPath != null)
                        SeanseManager.CopySeanses();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, "Ошибка при копировании сеансов");
                }
            });

            UpdateSeanses = new RelayCommand(() =>
            {
                try
                {
                    SeanseManager.UpdateSeanses();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, "Ошибка при обновлении сеансов");
                }
            });

            Thread updateWorker = new Thread(() =>
            {
                int updateCounter = 0;
                int copyCounter = 0;
                try {
                    while (true)
                    {
                        if (UpdateTimerOn && updateCounter == updateCounterLimit)
                        {
                            SeanseManager.UpdateSeanses();
                            updateCounter = 0;
                        }
                        if (CopyTimerOn && copyCounter == copyCounterLimit)
                        {
                            if (SeanseManager.DestinationPath != null)
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
                    System.Windows.MessageBox.Show(e.Message, "Ошибка в таймере");
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
