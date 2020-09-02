using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace chastocaBot_Telegram
{
    class LogHandler
    {
        public static void Log(string message,string textFrom)
        {
            string path = "E:\\chatLogs\\TelegramBot\\"+textFrom+ ".txt";
            
            File.AppendAllText(path, LogToFile(message,textFrom));
            LogToConsole(message,textFrom);
        }

        public static string LogToFile(string message, string textFrom)
        {   
            string logMessage = String.Format("{0} >>>> {1} : {2} {3}  ",
                DateTime.Now,
                textFrom,
                message,
                Environment.NewLine);

            return logMessage;
        }
        public static void LogToConsole(string message,string textFrom)
        {
            string logMessage = String.Format("{0} >>>> {1} : {2}  ",
                DateTime.Now,
                textFrom,
                message);
            Console.WriteLine(logMessage);
        }
    }
}
