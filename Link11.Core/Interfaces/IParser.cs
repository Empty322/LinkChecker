using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link11.Core.Interfaces
{
    public interface IParser
    {
        List<SignalEntry> Parse(List<string> lines);
    }
}
