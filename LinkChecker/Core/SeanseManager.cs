using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Logger;

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

        public SeanseManager(string destPath, ILogger logger)
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
            File.Delete("seanses.txt");
            foreach (Seanse s in Seanses)
            {
                File.AppendAllText("seanses.txt", s.Directory + Environment.NewLine);
            }
        }

        public List<Seanse> GetActiveSeanses()
        {
            List<Seanse> active = new List<Seanse>();
            foreach (Seanse seanse in Seanses)
                if (seanse.IsActive())
                    active.Add(seanse);
            return active;
        }

        #endregion
    }
}
