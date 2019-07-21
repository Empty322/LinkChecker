using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link11.Core.Charting
{
    public struct WorkingChartUnit
    {
        public DateTime Time { get; set; }
        public string StringTime { get { return Time.ToShortTimeString(); } }
        public int State { get; set; }
    }
}
