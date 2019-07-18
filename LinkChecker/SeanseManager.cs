using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Threading;
using Link11.Core.Interfaces;
using Link11.Core;
using Logger;
using Newtonsoft.Json;

namespace Link11Checker.Core
{
    public class SeanseManager
    {
        #region

        public event Action<object, Seanse> SeanseLoaded = (sender, seanse) => { };

        public event Action<object, Seanse> SeansesUpdated = (sender, e) => { };

        #endregion

        #region Properties

        public ObservableCollection<Seanse> Seanses { get; private set; }
        public string DestinationPath { get; set; }
        public bool UpdateTimerOn { get; set; }
        public bool CopyTimerOn { get; set; }

        #endregion

        #region Fields
    
        private ILogger logger;
        private Settings settings;

        #endregion

        #region Ctor

        public SeanseManager(Settings settings, IParser parser, ILogger logger)
        {
            this.Seanses = new ObservableCollection<Seanse>();
            this.logger = logger;
            this.UpdateTimerOn = false;
            this.CopyTimerOn = false;
            this.settings = settings;

            #region SetTimer

            Thread timer = new Thread(() =>
            {
                int updateCounter = 0;
                int copyCounter = 0;
                while (true)
                {
                    try
                    {
                        if (UpdateTimerOn && updateCounter >= settings.UpdateCounterLimit)
                        {
                            UpdateSeanses();
                            updateCounter = 0;
                        }
                        if (CopyTimerOn && copyCounter >= settings.CopyCounterLimit)
                        {
                            if (!string.IsNullOrWhiteSpace(DestinationPath))
                                CopySeanses();
                            copyCounter = 0;
                        }
                        updateCounter++;
                        copyCounter++;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        logger.LogMessage("Ошибка при копировании/обновлении. Папка с сеансом не найдена: \n" + e.Data["dir"].ToString(), LogLevel.Warning);
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                    Thread.Sleep(5000);
                }
            });

            timer.IsBackground = true;
            timer.Start();

            #endregion
        }

        #endregion

        #region Methods

        public void AddSeanse(Seanse seanse)
        {
            lock (Seanses)
            {
                Seanses.Add(seanse);
                SaveDirectories();
            }
        }

        public List<Seanse> GetSeansesFromVentursFile(string file)
        {
            List<Seanse> newSeanses = new List<Seanse>();
            lock (Seanses) {
                XmlSerializer ser = new XmlSerializer(typeof(ch));

                string[] channels = File.ReadAllLines(file, Encoding.Default);
                for (int i = 1; i < channels.Count(); i++)
                {
                    using (StringReader sr = new StringReader(channels[i]))
                    {
                        ch channel = (ch)ser.Deserialize(sr);
                        if (!Seanses.Select(x => x.Directory).Contains(channel.Directory) && (channel.Trakt == "slew" || channel.Trakt == "link11"))
                        {
                            try
                            {
                                Seanse newSeanse = new Seanse(channel.Directory);
                                SeanseLoaded.Invoke(this, newSeanse);
                                newSeanses.Add(newSeanse);
                                SaveDirectories();
                            }
                            catch (Seanse.LogFileNotFoundException e)
                            {
                                logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                            }                              
                        }
                    }
                }
            }
            return newSeanses;
        }
        public async Task<List<Seanse>> GetSeansesFromVentursFileAsync(string file)
        {
            Task<List<Seanse>> addingTask = Task.Run<List<Seanse>>(() => GetSeansesFromVentursFile(file));
            return await addingTask;
        }

        public List<Seanse> GetAllSeansesFromFolder(string path)
        {
            lock (Seanses)
            {
                List<Seanse> newSeanses = new List<Seanse>();
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] childDirs = di.GetDirectories();

                Thread t = Thread.CurrentThread;

                foreach (DirectoryInfo directory in childDirs)
                {
                    if (Seanses.Select(x => x.Directory).Contains(directory.FullName))
                        continue;
                    try
                    {
                        Seanse newSeanse = new Seanse(directory.FullName);
                        newSeanses.Add(newSeanse);
                        SeanseLoaded.Invoke(this, newSeanse);
                    }
                    catch (Seanse.LogFileNotFoundException e)
                    {
                        logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                    }
                }
                return newSeanses;
            }
        }

        public async Task<List<Seanse>> GetAllSeansesFromFolderAsync(string path)
        {
            Task<List<Seanse>> addingTask = Task.Run<List<Seanse>>(() => GetAllSeansesFromFolder(path));
            return await addingTask;
        }    

        public void RemoveSeanse(Seanse seanse)
        {
            lock (Seanses)
            {
                Seanses.Remove(seanse);
                SaveDirectories();
            }
        }

        public void RemoveAllSeanses()
        {
            lock (Seanses)
            {
                Seanses.Clear();
                SaveDirectories();
            }
        }

        public void CopySeanses()
        {
            lock (Seanses)
            {
                foreach (Seanse seanse in Seanses)
                    seanse.Copy(new DirectoryInfo(DestinationPath));
            }
        }

        public async Task CopySeansesAsync()
        {
            Task copyTask = Task.Run(() => CopySeanses());
            await copyTask;
        }

        public void UpdateSeanses()
        {
            lock (Seanses)
            {
                foreach (Seanse seanse in Seanses)
                {
                    try
                    {
                        seanse.Update();
                        SeansesUpdated.Invoke(this, seanse);
                    }
                    catch (Seanse.LogFileNotFoundException e)
                    {
                        logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                    }
                }
            }
        }
        public async Task UpdateSeansesAsync()
        {
            Task updateTask = Task.Run(() => UpdateSeanses());
            await updateTask;
        }

        private void SaveDirectories()
        {
            List<string> seansesToSave = new List<string>();
            foreach (Seanse s in Seanses)
            {
                seansesToSave.Add(s.Directory);
            };
            string json = JsonConvert.SerializeObject(seansesToSave);
            File.WriteAllText("seanses.json", json, Encoding.Default);
        }

        #endregion
    }
}
