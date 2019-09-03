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

        #endregion

        #region Ctor

        public SeanseManager(IParser parser, ILogger logger)
        {
            this.Seanses = new List<Seanse>();
            this.logger = logger;
            this.UpdateTimerOn = false;
            this.CopyTimerOn = false;

            Thread timer = new Thread(new ThreadStart(Timer));
            timer.IsBackground = true;
            timer.Start();
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

        public async Task RemoveSeanseAsync(Seanse seanse)
        {
            Task removingTask = Task.Run(() => RemoveSeanse(seanse));
            await removingTask;
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

        public async Task RemoveAllSeansesAsync()
        {
            Task removingTask = Task.Run(() => RemoveAllSeanses());
            await removingTask;
        }  

        private void RemoveExcessSeanses()
        {
            lock (Seanses)
            {
                List<ch> channels = GetChannelsFromVentursFile(IoC.Settings.VenturFile);
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

        public async Task RemoveExcessSeansesAsync()
        {
            Task removingTask = Task.Run(() => RemoveExcessSeanses());
            await removingTask;
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
                logger.LogMessage("=========================   ОБНОВЛЕНИЕ СЕАНСОВ   =========================", LogLevel.Info);
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
                        logger.LogMessage("Cеанс '" + seanse.Directory + "' не найден", LogLevel.Warning);
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.Message, LogLevel.Error);
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
            Seanse newSeanse = null;
            try
            {
                SeanseAdding.Invoke(this, directory);
                newSeanse = new Seanse(directory, IoC.Settings.Configuration);
                Seanses.Add(newSeanse);
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
            finally
            {
                SeanseAdded.Invoke(this, newSeanse);
            }
            return result;
        }

        #endregion

        public void SetConfiguration(Configuration configuration)
        {
            foreach (Seanse seanse in Seanses)
            {
                seanse.SetConfuguration(configuration);
            }
        }

        private async void Timer()
        {
            int updateCounter = 1;
            int copyCounter = 1;
            int synchronizeCounter = 1;
            while (true)
            {
                if (UpdateTimerOn && updateCounter >= IoC.Settings.UpdateCounterLimit)
                {
                    try
                    {
                        await UpdateSeansesAsync();
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                    updateCounter = 0;
                }
                if (CopyTimerOn && copyCounter >= IoC.Settings.CopyCounterLimit)
                {
                    if (!string.IsNullOrWhiteSpace(DestinationPath))
                        try
                        {
                            await CopySeansesAsync();
                        }
                        catch (Exception e)
                        {
                            logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                        }
                    copyCounter = 0;
                }
                if (SynchronyzeWithVenturOn && synchronizeCounter >= IoC.Settings.SynchronizeCounterLimit)
                {
                    try
                    {
                        await RemoveExcessSeansesAsync();
                        await AddSeansesFromVentursFileAsync(IoC.Settings.VenturFile);
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
                Thread.Sleep(1000);
            }
        }
    }
}
