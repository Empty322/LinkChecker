﻿using System;
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

        public event Action<object> LoadingStarted = (sender) => { };
        public event Action<object> LoadingEnded = (sender) => { };
        public event Action<object, string> SeanseAdding = (sender, direcory) => { };
        public event Action<object, Seanse> SeanseAdded = (sender, e) => { };
        public event Action<object, Seanse> SeanseUpdated = (sender, e) => { };
        public event Action<object, Seanse> SeanseRemoved = (sender, e) => { };
        public event Action<object> StartCopying = (sender) => { };
        public event Action<object> EndCopying = (sender) => { };
        public event Action<object> UpdatingStarted = (sender) => { };
        public event Action<object> UpdatingEnded = (sender) => { };
        public event Action<object> CopyingStarted = (sender) => { };
        public event Action<object, Seanse> SeanseCopyed = (sender, seanse) => { };
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
                LoadingStarted.Invoke(this);
                result = LoadSeanse(seanseDir);
                LoadingEnded.Invoke(this);
            }
            return result;
        }

        public async Task<bool> AddSeanseAsync(string seanseDir)
        {
            Task<bool> addingTask = Task<bool>.Run(() => AddSeanse(seanseDir));
            return await addingTask;
        }

        public void AddSeansesFromVentursFile(List<string> files)
        {
                LoadingStarted.Invoke(this);
                foreach (string file in files)
                {
                    List<Channel> channels = GetChannelsFromVentursFile(file);
                    Parallel.ForEach(channels, channelInfo =>
                    {
                        string pathPartToPaste = Path.GetDirectoryName(file);
                        string pathPartToDelete = channelInfo.Directory.Substring(0, channelInfo.Directory.IndexOf("riClient")+8);
                        string channelDirectory = channelInfo.Directory.Replace(pathPartToDelete, pathPartToPaste).ToLower();
                        Seanse currentSeanse = Seanses.FirstOrDefault(x => x.Directory.FullName.ToLower() == channelDirectory);
                        if (currentSeanse == null)
                        {
                            lock (Seanses)
                            {
                                LoadSeanse(channelDirectory);
                            }
                        }
                        if (currentSeanse != null)
                        {
                            currentSeanse.ChannelInfo = channelInfo;
                        }
                    });
                }
                LoadingEnded(this);
        }

        public async Task AddSeansesFromVentursFileAsync(List<string> files)
        {
            Task addingTask = Task.Run(() => AddSeansesFromVentursFile(files));
            await addingTask;
        }

        public void AddAllSeansesFromFolder(string path)
        {
            lock (Seanses)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] childDirs = di.GetDirectories();

                List<string> dirsInStock = Seanses.Select(x => x.Directory.FullName.ToLower()).ToList();
                LoadingStarted.Invoke(this);
                Parallel.ForEach(childDirs, directory =>
                {
                    if (!dirsInStock.Contains(directory.FullName.ToLower()))
                        LoadSeanse(directory.FullName);
                });
                LoadingEnded.Invoke(this);
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
            }
        }

        public async Task RemoveAllSeansesAsync()
        {
            Task removingTask = Task.Run(() => RemoveAllSeanses());
            await removingTask;
        }

        private void RemoveExcessSeanses(List<string> files)
        {
            lock (Seanses)
            {
                // Пути из файла last.lf
                List<string> ventursDirs = new List<string>();
                foreach (string file in files)
                {
                    List<Channel> channels = GetChannelsFromVentursFile(file);
                    string pathPartToPaste = Path.GetDirectoryName(file);
                    string pathPartToDelete = channels[0].Directory.Substring(0, channels[0].Directory.IndexOf("riClient") + 8);
                    string channelDirectory = channels[0].Directory.Replace(pathPartToDelete, pathPartToPaste).ToLower();
                    ventursDirs.AddRange(channels.Where(x => x.Trakt == "slew" || x.Trakt == "link11").Select(x => x.Directory.Replace(pathPartToDelete, pathPartToPaste).ToLower()).ToList());
                }
                // Список сеансов, не содержащихся в last.lf
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
                    {
                        SeanseRemoved.Invoke(this, seanseToRemove);
                    }
                }
            }
        }

        public async Task RemoveExcessSeansesAsync(List<string> files)
        {
            Task removingTask = Task.Run(() => RemoveExcessSeanses(files));
            await removingTask;
        }  

        public void CopySeanses()
        {
            lock (Seanses)
            {
                CopyingStarted.Invoke(this);
                Parallel.ForEach(Seanses, seanse =>
                {
                    bool result = false;
                    try
                    {
                        result = seanse.Copy(new DirectoryInfo(DestinationPath));
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.ToString() + " " + e.Message, LogLevel.Error);
                    }
                    finally
                    {
                        SeanseCopyed.Invoke(this, seanse);
                    }
                });
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
                Parallel.ForEach(Seanses, seanse =>
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
                });                
                UpdatingEnded.Invoke(this);
            }
        }
        public async Task UpdateSeansesAsync()
        {
            Task updateTask = Task.Run(() => UpdateSeanses());
            await updateTask;
        }

        private List<Channel> GetChannelsFromVentursFile(string file)
        {
            List<Channel> result = new List<Channel>();
            XmlSerializer ser = new XmlSerializer(typeof(Channel));
            try
            {
                string[] channels = File.ReadAllLines(file, Encoding.Default);
            
                for (int i = 1; i < channels.Count(); i++)
                {
                    using (StringReader sr = new StringReader(channels[i]))
                    {   
                        Channel channel = (Channel)ser.Deserialize(sr);
                        if (channel.Trakt == "link11" || channel.Trakt == "slew")
                        {
                            result.Add(channel);
                        }
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
                newSeanse = new Seanse(directory, IoCContainer.Settings.Configuration);
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
            lock (Seanses)
            {
                foreach (Seanse seanse in Seanses)
                {
                    seanse.SetConfuguration(configuration);
                }
            }
        }

        private async void Timer()
        {
            int updateCounter = 1;
            int copyCounter = 1;
            int synchronizeCounter = 1;
            while (true)
            {
                if (UpdateTimerOn && updateCounter >= IoCContainer.Settings.UpdateCounterLimit)
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
                if (CopyTimerOn && copyCounter >= IoCContainer.Settings.CopyCounterLimit)
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
                if (SynchronyzeWithVenturOn && synchronizeCounter >= IoCContainer.Settings.SynchronizeCounterLimit)
                {
                    try
                    {
                        await RemoveExcessSeansesAsync(IoCContainer.Settings.LastFiles);
                        await AddSeansesFromVentursFileAsync(IoCContainer.Settings.LastFiles);
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
