using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Logger;
using System.Diagnostics;
using Link11.Core;
using Newtonsoft.Json;

namespace Link11Report
{
    class Program
    {
        private static ILogger logger;
        public static bool LoadConfiguration(out Configuration config)
        {
            config = new Configuration();
            var a = Directory.GetCurrentDirectory();
            if (File.Exists("settings.json"))
            {
                try
                {
                    string settingsFile = File.ReadAllText("settings.json", Encoding.Default);
                    int start = settingsFile.IndexOf('{', 2);
                    int end = settingsFile.IndexOf('}', start) + 1;
                    string configFile = settingsFile.Substring(start, end - start);
                    config = JsonConvert.DeserializeObject<Configuration>(configFile);
                    return true;
                }
                catch (Exception e)
                {
                    logger.LogMessage("Не удалось загрузить файл конфигурации \n" + e.Message, LogLevel.Error);
                }
            }
            return false;
        }
        

        static void Main(string[] args)
        {
            logger = new PrimitiveLogger(Path.Combine(Directory.GetCurrentDirectory(), "report_log.txt"), LogLevel.Error);
            if (args.Count() < 2)
            {
                logger.LogMessage("Не указан путь к сеансу", LogLevel.Warning);
                return;
            }

            Configuration config = new Configuration();
            if (!LoadConfiguration(out config))
            {
                return;
            }

            int interval = 0;
            if (Int32.TryParse(args[0], out interval)) {
                for (int i = 1; i < args.Count(); i++)
                {
                    logger.LogMessage("Загрузка сеанса " + args[1], LogLevel.Info);
                    Seanse mySeanse = new Seanse(args[1], config);
                    logger.LogMessage("Составление отчета", LogLevel.Info);
                    List<ReportLine> report = mySeanse.GetReport(interval, config);

                    try
                    {
                        string reportFile = Path.Combine(args[i], String.Concat(mySeanse.Freq, '_', mySeanse.Mode, '_', interval, "report.csv"));
                        logger.LogMessage("Сохранение отчета в " + reportFile, LogLevel.Info);
                        File.WriteAllText(reportFile, report.ConvertToCSV());
                        Process excel = new Process();
                        excel.StartInfo = new ProcessStartInfo("excel", '"' + reportFile + '"');
                        excel.Start();
                    }
                    catch (Exception e)
                    {
                        logger.LogMessage(e.Message, LogLevel.Error);
                    }   
                }        
            }
            else {
                logger.LogMessage("Не удалось распознать интервал", LogLevel.Error);
            }
        }
    }
}
