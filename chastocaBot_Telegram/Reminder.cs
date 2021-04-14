using System;

namespace chastocaBot_Telegram
{
    class Reminder
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string WhoAdded { get; set; }
        public string Name { get; set; }
    }
}
