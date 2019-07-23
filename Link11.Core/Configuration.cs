using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link11.Core
{
    public struct Configuration
    {
        public double AbonentsK { get; set; }
        public double IntervalsK { get; set; }
        public int SmoothValue { get; set; }
        public int MinutesToAwaitAfterEnd { get; set; }
    }
}
