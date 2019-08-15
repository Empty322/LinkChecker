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
        #region Events

        public event Action<object, string> SeanseAdding = (sender, direcory) => { };
        public event Action<object, Seanse> SeanseUpdated = (sender, e) => { };
        public event Action<object, Seanse> SeanseAdded = (sender, e) => { };
        public event Action<object, Seanse> SeanseRemoved = (sender, e) => { };
        public event Action<object> StartCopying = (sender) => { };
        public event Action<object> EndCopying = (sender) => { };
        public event Action<object> UpdatingStarted = (sender) => { };
        public event Action<object> UpdatingEnded = (sender) => { };
        public event Action<object> CopyingStarted = (sender) => { };
        public event Action<object, int, int> SeanseCopyed = (sender, num, count) => { };
        public event Action<object> CopyingEnded = (sender) => { };
        

        #endregion

        #region Properties

        public List<Seanse> Seanses { get; private set; }
        public string DestinationPath { get; set; }
        public bool UpdateTimerOn { get; set; }
        public bool CopyTimerOn { get; set; }
        public bool SynchronyzeWithVenturOn { get; set; }

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
                int updateCounter = 1;
                int copyCounter = 1;
                int synchronizeCounter = 1;
                while (true)
                {
                    if (UpdateTimerOn && updateCounter >= settings.UpdateCounterLimit)
                    {
                        try
                        {
                            UpdateSeanses();
                        }
                        catch (Exception e)
                        {
                            logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        }
                        updateCounter = 0;
                    }
                    if (CopyTimerOn && copyCounter >= settings.CopyCounterLimit)
                    {
                        if (!string.IsNullOrWhiteSpace(DestinationPath))
                        try
                        {
                            CopySeanses();
                        }
                        catch (Exception e)
                        {
                            logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        }
                        copyCounter = 0;
                    }
                    if (SynchronyzeWithVenturOn && synchronizeCounter >= settings.SynchronizeCounterLimit)
                    {
                        try
                        {
                            AddSeansesFromVentursFile(settings.VenturFile);
                            RemoveExcessSeanses();
                        }
                        catch (Exception e)
                        {
                            logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        }
                        synchronizeCounter = 0;
                    }
                    updateCounter++;
                    copyCounter++;
                    synchronizeCounter++;
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
                result = LoadSeanse(seanseDir);
                SaveDirectories();
            }
            return result;
        }

        public async Task<bool> AddSeanseAsync(string seanseDir)
        {
            Task<bool> addingTask = Task<bool>.Run(() => AddSeanse(seanseDir));
            return await addingTask;
        }

        public void AddSeansesFromVentursFile(string file)
        {
            lock (Seanses) {
                List<ch> channels = GetChannelsFromVentursFile(file);
                foreach (ch channel in channels)
                {
                    if (!Seanses.Select(x => x.Directory.FullName.ToLower()).Contains(channel.Directory.ToLower()) && (channel.Trakt == "slew" || channel.Trakt == "link11"))
                    {
                        LoadSeanse(channel.Directory);
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

                List<string> dirsInStock = Seanses.Select(x => x.Directory.FullName.ToLower()).ToList();
                foreach (DirectoryInfo directory in childDirs)
                {
                    if (dirsInStock.Contains(directory.FullName.ToLower()))
                        continue;
                    LoadSeanse(directory.FullName);
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
                while (Seanses.Any())
                {
                    Seanse removedSeanse = Seanses[0];
                    Seanses.RemoveAt(0);
                    SeanseRemoved(this, removedSeanse);
                }
                SaveDirectories();
            }
        }

        private void RemoveExcessSeanses()
        {
            lock (Seanses)
            {
                List<ch> channels = GetChannelsFromVentursFile(settings.VenturFile);
                string[] ventursDirs = channels.Where(x => x.Trakt == "slew" || x.Trakt == "link11").Select(x => x.Directory.ToLower()).ToArray();
                List<Seanse> seansesToRemove = new List<Seanse>();
                foreach (Seanse seanse in Seanses)
                {
                    if (!ventursDirs.Contains(seanse.Directory.FullName.ToLower()))
                    {
                        seansesToRemove.Add(seanse);
                    }
                }
                foreach (Seanse seanseToRemove in seansesToRemove)
                {
                    if (Seanses.Remove(seanseToRemove))
                        SeanseRemoved.Invoke(this, seanseToRemove);
                }
            }
        }

        public void CopySeanses()
        {
            lock (Seanses)
            {
                CopyingStarted.Invoke(this);
                for (int i = 0; i < Seanses.Count; i++)
                {
                    bool result = false;
                    try
                    {
                        result = Seanses[i].Copy(new DirectoryInfo(DestinationPath));
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                    finally
                    {
                        SeanseCopyed.Invoke(this, i+1, Seanses.Count);
                    }
                }
                CopyingEnded.Invoke(this);
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
                UpdatingStarted.Invoke(this);
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
                    catch (DirectoryNotFoundException)
                    {
                        logger.LogMessage("Папка c сеансом " + seanse.Directory + " не найдена", LogLevel.Warning);
                    }
                }
                UpdatingEnded.Invoke(this);
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
                    seansesToSave.Add(s.Directory.FullName);
                };
                string json = JsonConvert.SerializeObject(seansesToSave);
                File.WriteAllText("seanses.json", json, Encoding.Default);
            }
        }

        private List<ch> GetChannelsFromVentursFile(string file)
        {
            List<ch> result = new List<ch>();
            XmlSerializer ser = new XmlSerializer(typeof(ch));
            try
            {
                string[] channels = File.ReadAllLines(file, Encoding.Default);
            
                for (int i = 1; i < channels.Count(); i++)
                {
                    using (StringReader sr = new StringReader(channels[i]))
                    {                
                        result.Add((ch)ser.Deserialize(sr));
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogMessage(e.ToString() + ' ' + e.Message, LogLevel.Error);
            }
            return result;
        }

        private bool LoadSeanse(string directory)
        {
            bool result = false;
            try
            {
                SeanseAdding.Invoke(this, directory);
                Seanse newSeanse = new Seanse(directory, settings.Configuration);
                Seanses.Add(newSeanse);
                SeanseAdded.Invoke(this, newSeanse);
                result = true;
            }
            catch (Seanse.LogFileNotFoundException e)
            {
                logger.LogMessage(e.FileName + " не найден", LogLevel.Warning);
            }
            catch (Exception e)
            {
                logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
            }
            return result;
        }

        #endregion
    }
}
