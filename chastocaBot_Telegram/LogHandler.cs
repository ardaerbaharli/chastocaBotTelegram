using System;
using System.IO;

namespace chastocaBot_Telegram
{
    class LogHandler
    {
        public static void Log(string message, string textFrom, string chatName)
        {
            if (chatName.Length < 1)
                chatName = "NO_USERNAME";

            string path = "D:\\chatLogs\\TelegramBot\\" + chatName + ".txt";

            File.AppendAllText(path, LogToFile(message, textFrom));
            LogToConsole(message, textFrom);
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
        public static void LogToConsole(string message, string textFrom)
        {
            string logMessage = String.Format("{0} >>>> {1} : {2}  ",
                DateTime.Now,
                textFrom,
                message);
            Console.WriteLine(logMessage);
        }
    }
}
