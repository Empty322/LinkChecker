using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Link11.Core.Interfaces;
using Link11.Core.Enums;

namespace Link11.Core
{
    public class Parser : IParser
    {
        public List<SignalEntry> Parse(List<string> lines)
        {
            List<SignalEntry> signal = new List<SignalEntry>();
            for (int i = 0; i < lines.Count(); i++)
            {
                string[] lineData = lines[i].Split('\t');
                SignalEntry se = new SignalEntry();
                
                int num = 0;
                DateTime time;
                int abonent = 0;
                int size = 0;
                int errors = 0;
                int nInterval = 0;
                int kInterval = 0;

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

                        // Уровень

                        signal.Add(se);
                    }
                }
            }
            return signal;
        }
    }
}
