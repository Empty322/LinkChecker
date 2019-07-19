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
        public event Action<object, Seanse> SeanseUpdated = (sender, e) => { };
        public event Action<object, Seanse> SeanseAdded = (sender, e) => { };
        public event Action<object, Seanse> SeanseRemoved = (sender, e) => { };
        public event Action<object> StartCopying = (sender) => { };
        public event Action<object> EndCopying = (sender) => { };
        public event Action<object> StartUpdating = (sender) => { };
        public event Action<object> EndUpdating = (sender) => { };
        

        #endregion

        #region Properties

        public List<Seanse> Seanses { get; private set; }
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
            this.Seanses = new List<Seanse>();
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

        public bool AddSeanse(string seanseDir)
        {
            bool result = false;
            lock (Seanses)
            {
                try
                {
                    Seanse newSeanse = new Seanse(seanseDir, settings.Configuration);
                    Seanses.Add(newSeanse);
                    SeanseAdded.Invoke(this, newSeanse);
                    SaveDirectories();
                    result = true;
                }
                catch (DirectoryNotFoundException e)
                {
                    logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                }
                catch (Seanse.LogFileNotFoundException e)
                {
                    logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                }
            }
            return result;
        }

        public void AddSeansesFromVentursFile(string file)
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
                        if (!Seanses.Select(x => x.Directory.ToLower()).Contains(channel.Directory.ToLower()) && (channel.Trakt == "slew" || channel.Trakt == "link11"))
                        {
                            try
                            {
                                Seanse newSeanse = new Seanse(channel.Directory, settings.Configuration);
                                Seanses.Add(newSeanse);
                                SeanseAdded.Invoke(this, newSeanse);
                            }
                            catch (Seanse.LogFileNotFoundException e)
                            {
                                logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                            }
                            catch (Exception e)
                            {
                                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                            }
                        }
                    }
                }
                SaveDirectories();
            }
        }

        public async Task AddSeansesFromVentursFileAsync(string file)
        {
            Task addingTask = Task.Run(() => AddSeansesFromVentursFile(file));
            await addingTask;
        }

        public void AddAllSeansesFromFolder(string path)
        {
            lock (Seanses)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] childDirs = di.GetDirectories();

                List<string> dirsInStock = Seanses.Select(x => x.Directory.ToLower()).ToList();
                foreach (DirectoryInfo directory in childDirs)
                {
                    if (dirsInStock.Contains(directory.FullName.ToLower()))
                        continue;
                    try
                    {
                        Seanse newSeanse = new Seanse(directory.FullName, settings.Configuration);
                        Seanses.Add(newSeanse);
                        SeanseAdded.Invoke(this, newSeanse);
                    }
                    catch (Seanse.LogFileNotFoundException e)
                    {
                        logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                }
                SaveDirectories();
            }
        }

        public async Task AddAllSeansesFromFolderAsync(string path)
        {
            Task addingTask = Task.Run(() => AddAllSeansesFromFolder(path));
            await addingTask;
        }    

        public void RemoveSeanse(Seanse seanse)
        {
            lock (Seanses)
            {
                if (Seanses.Remove(seanse))
                {
                    SeanseRemoved(this, seanse);
                    SaveDirectories();
                }
            }
        }

        public void RemoveAllSeanses()
        {
            lock (Seanses)
            {
                while (Seanses.Count > 0)
                {
                    Seanse removedSeanse = Seanses[0];
                    Seanses.RemoveAt(0);
                    SeanseRemoved(this, removedSeanse);
                }
                SaveDirectories();
            }
        }

        public void CopySeanses()
        {
            lock (Seanses)
            {
                foreach (Seanse seanse in Seanses)
                {
                    try
                    {
                        seanse.Copy(new DirectoryInfo(DestinationPath));
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                }
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
                        SeanseUpdated.Invoke(this, seanse);
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
            lock (Seanses)
            {
                List<string> seansesToSave = new List<string>();
                foreach (Seanse s in Seanses)
                {
                    seansesToSave.Add(s.Directory);
                };
                string json = JsonConvert.SerializeObject(seansesToSave);
                File.WriteAllText("seanses.json", json, Encoding.Default);
            }
        }

        #endregion
    }
}
