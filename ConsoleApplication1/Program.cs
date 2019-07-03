using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11Core;
using System.IO;
using Link11Checker.Core;

namespace ConsoleApplication1
{
    class Program
    {

        static void Main(string[] args)
        {
            Signal signal = new Signal(new Parser(), new Configuration { AbonentsK = 0.1, IntervalsK = 0.05 });
            DateTime time = DateTime.Parse("19:00");
            signal.LoadSignal(@"..\..\..\..\46report\Link11Core.Test\parserInput.txt");
            Seanse s = new Seanse(@"d:\Bespalov\logs\4548.7__10_50_12\");
            s.IsWorking(DateTime.Parse("23:00"), DateTime.Parse("1:00"));
        }
    }
}
