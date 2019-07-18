using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11.Core.Enums;

namespace Link11.Core.Interfaces
{
    public interface IParser
    {
        List<SignalEntry> ParseLog(List<string> lines);
        void ParseAllLog(string content, out float freq, out Mode mode);
    }
}
