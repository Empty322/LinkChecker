using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Link11.Core.Interfaces;
using Link11.Core;
using Logger;
using Newtonsoft.Json;

namespace Link11Checker.Core
{
    public class SeanseManager
    {
        #region Properties

        public ObservableCollection<Seanse> Seanses { get; private set; }
        public string DestinationPath { get; set; }

        #endregion

        #region Fields
    
        private ILogger logger;

        #endregion

        #region Ctor

        public SeanseManager(string destPath) : this(destPath, new Configuration { AbonentsK = 0.15, IntervalsK = 0.2 }, new Parser(), new PrimitiveLogger(LogLevel.Error)) { }

        public SeanseManager(string destPath, Configuration config, IParser parser, ILogger logger)
        {
            this.DestinationPath = destPath;
            this.Seanses = new ObservableCollection<Seanse>();
            this.logger = logger;
        }

        #endregion

        #region Methods

        public void AddSeanse(Seanse seanse)
        {
            Seanses.Add(seanse);
            SaveDirectories();
        }

        public void RemoveSeanse(Seanse seanse)
        {
            Seanses.Remove(seanse);
            SaveDirectories();
        }

        public void CopySeanses()
        {
            foreach (Seanse seanse in Seanses)
                seanse.Copy(new DirectoryInfo(DestinationPath));
        }

        public void UpdateSeanses()
        {
            foreach (Seanse seanse in Seanses)
            {
                seanse.Update();
            }
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

        //public List<Seanse> GetActiveSeanses()
        //{
        //    List<Seanse> active = new List<Seanse>();
        //    foreach (Seanse seanse in Seanses)
        //        if (seanse.IsActive())
        //            active.Add(seanse);
        //    return active;
        //}

        #endregion
    }
}
