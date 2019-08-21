using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link11Report
{
    public class ReportLine
    {
        public DateTime Time { get; set; }
        public int Abonents { get; set; }
        public int AbonentsWithInterval { get; set; }
        public int MaxSizeInFrames { get; set; }
        public double MaxSizeInBytes { get; set; }
    }
}
