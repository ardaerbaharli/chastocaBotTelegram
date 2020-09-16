using System;
using System.IO;

namespace chastocaBot_Telegram
{
    class LogHandler
    {
        public static void Log(Log log)
        {
            if (log.LogName == null)
                log.LogName= "NO_LOG_NAME";

            string path = "D:\\chatLogs\\TelegramBot\\" + log.LogName + ".txt";

            File.AppendAllText(path, LogToFile(log));
            LogToConsole(log);
        }

        public static string LogToFile(Log log)
        {
            string logMessage = String.Format("{0} >>>> {1} : {2} {3}  ",
                DateTime.Now,
                log.Sender,
                log.Message,
                Environment.NewLine);

            return logMessage;
        }
        public static void LogToConsole(Log log)
        {
            string logMessage = String.Format("{0} >>>> {1} : {2}  ",
                DateTime.Now,
                log.Sender,
                log.Message);
            Console.WriteLine(logMessage);
        }
    }
}
