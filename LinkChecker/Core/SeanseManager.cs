using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Link11Checker.Core
{
    public class SeanseManager
    {
        #region Properties

        public List<Seanse> Seanses { get; private set; }
        public string DestinationPath { get; set; }

        #endregion

        #region Fields



        #endregion

        #region Ctor

        public SeanseManager() : this("") { }

        public SeanseManager(string destPath)
        {
            DestinationPath = destPath;
            Seanses = new List<Seanse>();
        }

        #endregion

        #region Methods

        public void AddSeanse(Seanse seanse)
        {
            Seanses.Add(seanse);
        }

        public void RemoveSeanse(Seanse seanse)
        {
            Seanses.Remove(seanse);
        }

        public void CopySeanses()
        {
            foreach (Seanse seanse in Seanses)
                seanse.Copy(new DirectoryInfo(DestinationPath));
        }

        public void UpdateSeanses()
        {
            foreach (Seanse seanse in Seanses)
                seanse.Update();
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
