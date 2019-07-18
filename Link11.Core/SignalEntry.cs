using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11.Core.Enums;

namespace Link11.Core
{
    public class SignalEntry
    {
        public int Number { get; set; }
        public DateTime Time { get; set; }
        public Source Source { get; set; }
        public EntryType Type { get; set; }
        public int? Abonent { get; set; }
        public int Size { get; set; }
        public int Errors { get; set; }
        public int Ninterval { get; set; }
        public int Kinterval { get; set; }
        public int Ext { get; set; }
        public float Tuning { get; set; }
        public float Level { get; set; }
    }
}
