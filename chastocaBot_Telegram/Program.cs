using System;

namespace chastocaBot_Telegram
{
    class Program
    {       
        static void Main(string[] args)
        {
            BotControl bc = new BotControl();
            bc.Connect();
           
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            bc.Disconnect();
        }      
    }
}
