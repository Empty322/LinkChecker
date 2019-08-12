using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11.Core;

namespace Link11Checker.Core
{
    public struct Settings
    {
        public Configuration Configuration { get; set; }
        public string InitialSeansesPath { get; set; }
        public string InitialDestPath { get; set; }
        public string VenturFile { get; set; }
        public int UpdateCounterLimit { get; set; }
        public int CopyCounterLimit { get; set; }
        public int SynchronizeCounterLimit { get; set; }
        public int WorkingChartInterval { get; set; }
        public bool HideEmptySeanses { get; set; }
    }
}
