using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Logger
{
    public class PrimitiveLogger : ILogger 
    {
        private LogLevel level;
        private FileInfo logFile;
        private bool logInFile;

        public PrimitiveLogger(LogLevel level) : this("log.txt", level) 
        {
            logInFile = false;
        }

        public PrimitiveLogger(string path, LogLevel level)
        {
            logInFile = true;
            logFile = new FileInfo(path);
            if (!Directory.Exists(logFile.Directory.FullName))
                Directory.CreateDirectory(logFile.Directory.FullName);
            if (!File.Exists(logFile.FullName))
                File.Create(logFile.FullName);
            this.level = level;
        }

        public void LogMessage(string message, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }

            message = DateTime.Now.ToString() + '\t' + level.ToString() + "\t" + message;
            if (level >= this.level)
            {
                if (logInFile)
                {
                    File.AppendAllText(logFile.FullName, message + '\n', Encoding.Default);
                }                
                Console.WriteLine(message);
            }
            Console.ForegroundColor = ConsoleColor.Red;
        }
    }
}
