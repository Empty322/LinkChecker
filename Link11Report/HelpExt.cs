using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Link11.Core;

namespace Link11Report
{
    public static class HelpExt
    {
        public static string ConvertToCSV(this List<ReportLine> report)
        {
            StringBuilder result = new StringBuilder();
            foreach (ReportLine line in report)
            {
                result.Append(line.Time.TimeOfDay.ToString().Substring(0, 5)).Append(';')
                    .Append(line.Abonents).Append(';')
                    .Append(line.AbonentsWithInterval).Append(';')
                    .Append(line.MaxSizeInFrames).Append(';')
                    .Append(line.MaxSizeInBytes).Append('\n');
            }
            return result.ToString();
        }

        public static List<ReportLine> GetReport(this Seanse seanse, int interval, Configuration config) {
            DateTime currentTime = seanse.StartWorkingTime;
            List<ReportLine> report = new List<ReportLine>();
            DateTime end = seanse.LastWorkingTime;

            while (currentTime < end)
            {
                ReportLine line = new ReportLine();

                DateTime nextTime = currentTime.AddMinutes(60 - currentTime.Minute);
                if (nextTime > end)
                    nextTime = end;
                line.Time = nextTime;

                line.Abonents = seanse.GetAbonents(currentTime, nextTime, config.AbonentsK).Count;
                line.AbonentsWithInterval = seanse.GetAbonentsWithInterval(currentTime, nextTime, interval, 0.15f, 0.2f).Count;
                line.MaxSizeInFrames = seanse.GetMaxInFrames(currentTime, nextTime);
                line.MaxSizeInBytes = line.MaxSizeInFrames * 3.75f;

                report.Add(line);

                currentTime = currentTime.AddMinutes(60 - currentTime.Minute);
            }

            return report;
        }
    }
}
