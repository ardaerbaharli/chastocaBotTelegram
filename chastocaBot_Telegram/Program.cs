using System;

namespace chastocaBot_Telegram
{
    class Program
    {
        private static void Main()
        {
            Console.Title = "chastocaBot-Telegram";
            BotControl bc = new BotControl();
            bc.Connect();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            bc.Disconnect();
        }
    }
}
