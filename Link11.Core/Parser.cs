using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11.Core.Interfaces;
using Link11.Core.Enums;
using Logger;

namespace Link11.Core
{
    public class Parser : IParser
    {

        private ILogger logger;

        public Parser() : this(new PrimitiveLogger("log.txt", LogLevel.Error)) { }

        public Parser(ILogger logger)
        {
            this.logger = logger;
        }

        public List<SignalEntry> ParseLog(List<string> lines)
        {
            List<SignalEntry> signal = new List<SignalEntry>();
            for (int i = 0; i < lines.Count(); i++)
            {
                string[] lineData = lines[i].Split('\t');
                SignalEntry se = new SignalEntry();
                int dayNumber = 0;

                int num = 0;
                DateTime time;
                int abonent = 0;
                int size = 0;
                int errors = 0;
                int nInterval = 0;
                int kInterval = 0;
                float tuning = 0;

                // Номер
                if (Int32.TryParse(lineData[0], out num))
                {
                    se.Number = num;
                    // Время
                    if (DateTime.TryParse(lineData[1], out time)) {
                        se.Time = time;

                        // Источник
                        if (lineData[2] == "Глав. Ст.")
                            se.Source = Source.MainStation;
                        else
                            se.Source = Source.Other;

                        // Тип
                        if (lineData[4] == "Ошибка")
                        {
                            se.Type = EntryType.Error;
                        }
                        else
                        {
                            switch (lineData[3])
                            {
                                case "Вызов    ":
                                    se.Type = EntryType.Call;
                                    break;
                                case "Ответ    ":
                                    se.Type = EntryType.Answer;
                                    break;
                                case "Сообщение":
                                    se.Type = EntryType.Message;
                                    break;
                                default:
                                    se.Type = EntryType.None;
                                    logger.LogMessage("ОШИБКА ПАРСЕРА: не удалось определить тип вхождения.", LogLevel.Error);
                                    break;
                            }
                        }                        

                        // Абонент
                        if (Int32.TryParse(lineData[4], out abonent)) {
                            se.Abonent = abonent;
                        }
                        else {
                            se.Abonent = null;
                        }

                        // Объем
                        if (Int32.TryParse(lineData[5], out size)) {
                            se.Size = size;
                        }
                        else {
                            se.Size = 0;
                        }

                        // Ошибки
                        if (Int32.TryParse(lineData[6], out errors)) {
                            se.Errors = errors;
                        }

                        // Н-Интервал
                        if (Int32.TryParse(lineData[7], out nInterval))
                        {
                            se.Ninterval = nInterval;
                        }

                        // К-Интервал
                        if (Int32.TryParse(lineData[8], out kInterval))
                        {
                            se.Kinterval = kInterval;
                        }

                        // Скорость/Длина

                        // Расстройка
                        if (lineData[10].Count() > 2) {
                            string value = lineData[10].Substring(1);
                            if (float.TryParse(value.Replace('.', ','), out tuning))
                                se.Tuning = tuning;
                            if (lineData[10][0] == '-')
                                se.Tuning = -se.Tuning;
                        }
                        
                        // Корректирование даты
                        if (signal.Any() && signal.Last().Time > se.Time)
                            dayNumber++;
                        se.Time = se.Time.AddDays((double)dayNumber);

                        signal.Add(se);
                    }
                }
            }
                        
            return signal;
        }

        public void ParseAllLog(string content, out float freq, out Mode mode)
        {
            // Режим
            string modeString = content.Substring(11, 4);
            if (modeString == "CLEW")
                mode = Mode.Clew;
            else if (modeString == "SLEW")
                mode = Mode.Slew;
            else
                mode = Mode.Unknown;
            // Частота
            freq = float.Parse(content.Substring(25, 6).Replace('.', ','));
        }
    }
}
