using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace chastocaBot_Telegram
{
    class BotControl
    {
        private static readonly string botToken = "1280937266:AAHK0ETudaO0yPXttoEAiCoLn6Ypgc0dyZg";
        static ITelegramBotClient botClient;
        private static Reminder soonestReminder;
        internal void Connect()
        {
            DatabaseHandler.CreateTables();
            botClient = new TelegramBotClient(botToken);
            var bot = botClient.GetMeAsync().Result;
            bot.CanJoinGroups = true;
            bot.CanReadAllGroupMessages = true;
            Console.WriteLine(bot.Username + " is online!");

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            soonestReminder = DatabaseHandler.GetSoonestReminder();

            Timer timer = new System.Timers.Timer(1000 * 45);
            timer.Elapsed += (sender, e) => ReminderController(sender, e);
            timer.Enabled = true;
            timer.Start();
        }

        private async void ReminderController(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Month == soonestReminder.Date.Month
                && soonestReminder.Date.Year == soonestReminder.Date.Year
                && DateTime.Now.Day == soonestReminder.Date.Day
                && DateTime.Now.Hour == soonestReminder.Date.Hour
                && DateTime.Now.Minute == soonestReminder.Date.Minute)
            {
                DatabaseHandler.DeleteReminder(soonestReminder);
                string announce = string.Format("*REMINDER*\n"+
                                           "*Name:* {0}\n" +
                                           "*Message:* {1}\n" +
                                           "*Time:* {2}"
                                           , soonestReminder.Name, soonestReminder.Text, soonestReminder.Date);
                await SendMessage(announce, soonestReminder.WhoAdded);
                soonestReminder = DatabaseHandler.GetSoonestReminder();
            }
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {

            string botAnswer = "";
            string botAnswer2 = "";
            string message = e.Message.Text;
            if (!string.IsNullOrEmpty(message))
            {
                string[] fragmentedMessage = message.Split(' ', ':', ';', '\t');
                string username = e.Message.From.Username;
                string chatId = e.Message.Chat.Id.ToString();
                string botName = "chastocaBot";
                string announce = "";
                string admin = "ardaerbaharli";
                if (message != null)
                {
                    Log log = new Log
                    {
                        Message = message,
                        Sender = username,
                        LogName = username
                    };
                    //  LogHandler.Log(log);
                    if (username != null)
                    {
                        if (message.Equals("/start"))
                        {
                            User newUser = new User
                            {
                                Username = username,
                                ChatId = chatId,
                            };
                            DatabaseHandler.AddUser(newUser);
                            botAnswer = GetCommands(username);
                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                        }
                        else if (DatabaseHandler.DoesExistInUsers(username, chatId))
                        {
                            User currentUser = DatabaseHandler.GetUser(username);

                            if (!message.Contains("'"))
                            {
                                if (fragmentedMessage[0].Equals("!feedback"))
                                {
                                    if (message.Length > 12)
                                    {
                                        string feedback = message.Substring(fragmentedMessage[0].Length + 1); //1 for space
                                        log.Message = feedback;
                                        log.Sender = username;
                                        log.LogName = "FEEDBACKS";
                                        LogHandler.Log(log);
                                        botAnswer = "Thank you for your feedback!";
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                }
                                else if ((fragmentedMessage[0].Equals("!addlocation") || fragmentedMessage[0].Equals("!addloc")))  //!addlocation arda lati longi
                                {
                                    if (fragmentedMessage.Length == 4)
                                    {
                                        if (fragmentedMessage[2].Contains('.') && fragmentedMessage[3].Contains('.'))
                                        {
                                            Location location = new Location
                                            {
                                                WhoAdded = currentUser.Username,
                                                Name = fragmentedMessage[1],
                                                Latitude = float.Parse(fragmentedMessage[2], CultureInfo.InvariantCulture.NumberFormat),
                                                Longitude = float.Parse(fragmentedMessage[3], CultureInfo.InvariantCulture.NumberFormat)
                                            };

                                            bool isSuccessful = DatabaseHandler.AddLocation(location);
                                            botAnswer = "Location added: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                        else
                                        {
                                            botAnswer = "Latitude and longitude values should be like: 39.895613 32.798303 ";
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                    }
                                    else
                                    {
                                        botAnswer2 = "!addlocation <locationName> <latitude> <longitude>\nLatitude and longitude values should be like: 39.895613 32.798303";
                                        char x = botAnswer2.ElementAt(22);
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer2, ParseMode.Markdown);
                                    }
                                }
                                else if ((fragmentedMessage[0].Equals("!deletelocation") || fragmentedMessage[0].Equals("!delloc")))
                                {
                                    if (fragmentedMessage.Length == 2)
                                    {
                                        string name = fragmentedMessage[1];
                                        Location location = DatabaseHandler.GetLocation(name, username);
                                        if (location.WhoAdded == username)
                                        {
                                            bool isSuccessful = DatabaseHandler.DeleteLocation(name, username);
                                            botAnswer = "Location deleted: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!deletelocation <name>", ParseMode.Markdown);
                                }
                                else if ((message.Equals("!locations") || message.Equals("!l")))
                                {
                                    var locations = DatabaseHandler.GetLocationNamesFrom(username);
                                    var buttons = locations.Select(location => new[] { new KeyboardButton(location) })
                                        .ToArray();
                                    var selections = new ReplyKeyboardMarkup(buttons);

                                    await botClient.SendTextMessageAsync(e.Message.Chat,
                                        "*Select a location.*",
                                        ParseMode.Markdown,
                                        replyToMessageId: e.Message.MessageId,
                                        replyMarkup: selections);
                                }
                                else if ((fragmentedMessage[0].Equals("!addcommand") || fragmentedMessage[0].Equals("!addcom")))
                                {
                                    if (fragmentedMessage.Length > 2)
                                    {
                                        Command command = new Command();
                                        command.Question = fragmentedMessage[1];
                                        command.WhoAdded = username;
                                        int startOffset = command.Question.Length + fragmentedMessage[0].Length + 2; // 2 for spaces
                                        command.Reply = message[startOffset..message.Length].Trim();

                                        bool isSuccessful = DatabaseHandler.AddCommand(command);
                                        botAnswer = "Command added: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!addcommand <command> <reply>", ParseMode.Markdown);
                                }
                                else if ((fragmentedMessage[0].Equals("!deletecommand") || fragmentedMessage[0].Equals("!delcom")))
                                {
                                    string fromWho = currentUser.Username;

                                    if (fragmentedMessage.Length == 2)
                                    {
                                        string question = fragmentedMessage[1];
                                        var command = DatabaseHandler.GetCommandFrom(question, fromWho);
                                        if (command != null)
                                        {
                                            bool isSuccessful = DatabaseHandler.DeleteCommand(command);
                                            botAnswer = "Command deleted: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                        else
                                            botAnswer = "The command doesn't exist or you don't have a permission to delete that command.";
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecommand <command>", ParseMode.Markdown);

                                }
                                else if ((fragmentedMessage[0].Equals("!updatecommand") || fragmentedMessage[0].Equals("!upcom")))
                                {
                                    if (fragmentedMessage.Length > 2)
                                    {
                                        Command command = new Command();
                                        command.Question = fragmentedMessage[1];
                                        Command oldCommand = DatabaseHandler.GetCommandFrom(command.Question, currentUser.Username);
                                        command.WhoAdded = oldCommand.WhoAdded;
                                        int startOffset = command.Question.Length + fragmentedMessage[0].Length + 2; //2 for spaces
                                        command.Reply = message[startOffset..message.Length].Trim();

                                        if (oldCommand != null)
                                        {
                                            bool isSuccessful = DatabaseHandler.UpdateCommand(command);
                                            botAnswer = "Command updated: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                        else
                                            await botClient.SendTextMessageAsync(e.Message.Chat, "You don't have permission to change this command.", ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!updatecommand <command> <newReply>", ParseMode.Markdown);
                                }
                                else if ((message.Equals("!commands") || message.Equals("!help") || message.Equals("!c")))
                                {
                                    botAnswer = GetCommands(username);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                }
                                else if ((fragmentedMessage[0].Equals("!addcocktail") && username == admin)) //!addcocktail name type recipe
                                {
                                    if (fragmentedMessage.Length > 3)
                                    {
                                        Cocktail cocktail = new Cocktail();
                                        cocktail.Name = fragmentedMessage[1];
                                        cocktail.Type = fragmentedMessage[2];
                                        int startOffset = cocktail.Name.Length + cocktail.Type.Length + fragmentedMessage[0].Length + 3;
                                        cocktail.Recipe = message[startOffset..message.Length].Trim();

                                        DatabaseHandler.AddCocktailType(cocktail.Type);
                                        bool isSuccessful = DatabaseHandler.AddCocktail(cocktail);
                                        botAnswer = "Cocktail added: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!addcocktail <cocktailName> <cocktailtype> <recipe>", ParseMode.Markdown);
                                }
                                else if ((fragmentedMessage[0].Equals("!deletecocktail") && username == admin))
                                {
                                    if (fragmentedMessage.Length == 2)
                                    {
                                        string cocktailName = fragmentedMessage[1];
                                        bool isSuccessful = DatabaseHandler.DeleteCocktail(cocktailName);
                                        botAnswer = "Cocktail deleted: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecocktail <cocktailName>", ParseMode.Markdown);
                                }
                                else if ((fragmentedMessage[0].Equals("!requestcocktail")))
                                {
                                    if (fragmentedMessage.Length > 1)
                                    {
                                        int startOffset = fragmentedMessage[0].Length + 1;
                                        string cocktailName = message[startOffset..message.Length].Trim();
                                        Log cocktailLog = new Log
                                        {
                                            LogName = "COCKTAIL REQUESTS",
                                            Message = cocktailName,
                                            Sender = username
                                        };
                                        LogHandler.Log(cocktailLog);
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "We will add the requested cocktail as soon as possible.", ParseMode.Markdown);
                                        announce = string.Format("{0} requested {1} named cocktail to be added to the cocktails.", username, cocktailName);
                                        await SendMessage(announce, admin);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!requestcocktal <cocktailName>", ParseMode.Markdown);
                                }
                                else if ((message.Equals("!users") || message.Equals("!u")))
                                {
                                    List<User> Users = DatabaseHandler.GetUsers();
                                    botAnswer = "*--Username--*";
                                    foreach (var user in Users)
                                        botAnswer += string.Format("\n{0}", user.Username);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                }
                                else if ((fragmentedMessage[0].Equals("!brute") || fragmentedMessage[0].Equals("!b")) && username == admin)
                                {
                                    if (fragmentedMessage.Length > 2)
                                    {
                                        string toWho = fragmentedMessage[1];
                                        int howMany = Int32.Parse(fragmentedMessage[2]);
                                        int startOffset = fragmentedMessage[2].Length + fragmentedMessage[0].Length + fragmentedMessage[1].Length + 3;
                                        announce = string.Format("{0} spamming you {1} times.", username, howMany);
                                        await SendMessage(announce, toWho);
                                        announce = message[startOffset..message.Length].Trim();
                                        bool isSuccessful = await BruteAnnounce(announce, toWho, howMany);
                                        botAnswer = "Announce successfull: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!brute <toWho> <howMany> <message>", ParseMode.Markdown);
                                    }
                                }
                                else if (message.Equals("!cocktails"))
                                {
                                    var cocktailTypes = DatabaseHandler.GetCocktailTypes();
                                    var buttons = cocktailTypes.Select(cocktailType => new[] { new KeyboardButton(cocktailType) })
                                        .ToArray();
                                    var selections = new ReplyKeyboardMarkup(buttons);

                                    await botClient.SendTextMessageAsync(e.Message.Chat,
                                        "*Select a cocktail type to see cocktails.*",
                                        ParseMode.Markdown,
                                        replyToMessageId: e.Message.MessageId,
                                        replyMarkup: selections);
                                }
                                else if (fragmentedMessage[0].Equals("!notifyme"))
                                {
                                    bool isFormatCorrect = true;
                                    if (fragmentedMessage.Length > 4)
                                    {
                                        Reminder reminder = new Reminder();
                                        reminder.WhoAdded = username;
                                        reminder.Name = fragmentedMessage[1];
                                        try
                                        {
                                            var y = DateTime.Now.Year;
                                            var m = Int32.Parse(fragmentedMessage[2].Substring(3, 2));
                                            var d = Int32.Parse(fragmentedMessage[2].Substring(0, 2));
                                            var h = Int32.Parse(fragmentedMessage[3]);
                                            var min = Int32.Parse(fragmentedMessage[4]);
                                            reminder.Date = new DateTime(y, m, d, h, min, 0);
                                        }
                                        catch (Exception ex)
                                        {
                                            isFormatCorrect = false;
                                            LogHandler.ReportCrash(ex);
                                            botAnswer = "!notifyme <Reminder_Name> <DD.MM> <HH:MM> <text>";

                                        }
                                        if (isFormatCorrect)
                                        {
                                            int startOffset = "!notifyme DD.MM HH:MM ".Length + reminder.Name.Length;
                                            reminder.Text = message[startOffset..message.Length].Trim();

                                            if (DateTime.Compare(reminder.Date, DateTime.Now) > 0)
                                            {
                                                bool isSuccessful = DatabaseHandler.AddReminder(reminder);
                                                botAnswer = "Reminder added: " + isSuccessful;

                                                soonestReminder = DatabaseHandler.GetSoonestReminder();
                                            }
                                            else
                                                botAnswer = "You can't set a reminder earlier than now.";
                                        }
                                    }
                                    else
                                        botAnswer = "!notifyme <ReminderName> <DD.MM> <HH:MM> <text>";

                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);

                                }
                                else if (message.Equals("!reminders"))
                                {
                                    var reminders = DatabaseHandler.GetRemindersFrom(username).Select(x => x.Name).ToArray(); ;
                                    var buttons = reminders.Select(reminder => new[] { new KeyboardButton(reminder.ToString()) })
                                       .ToArray();
                                    var selections = new ReplyKeyboardMarkup(buttons);

                                    await botClient.SendTextMessageAsync(e.Message.Chat,
                                        "*Select a reminder time to see details.*",
                                        ParseMode.Markdown,
                                        replyToMessageId: e.Message.MessageId,
                                        replyMarkup: selections);
                                }
                                else if ((fragmentedMessage[0].Equals("!deletereminder") || fragmentedMessage[0].Equals("!delrem")))
                                {
                                    string whoAdded = currentUser.Username;

                                    if (fragmentedMessage.Length == 2)
                                    {
                                        string reminderName = fragmentedMessage[1];
                                        Reminder reminder = DatabaseHandler.GetReminder(reminderName, whoAdded);
                                        if (reminder != null)
                                        {
                                            bool isSuccessful = DatabaseHandler.DeleteReminder(reminder);
                                            botAnswer = "Reminder deleted: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                        else
                                            botAnswer = "The reminder doesn't exist or you don't have a permission to delete that reminder.";
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!deletereminder <reminderName>", ParseMode.Markdown);

                                }
                                else if (message.Equals("Go Back"))
                                {
                                    var cocktailTypes = DatabaseHandler.GetCocktailTypes();
                                    var buttons = cocktailTypes.Select(cocktailType => new[] { new KeyboardButton(cocktailType) })
                                        .ToArray();
                                    var selections = new ReplyKeyboardMarkup(buttons);

                                    await botClient.SendTextMessageAsync(e.Message.Chat,
                                        "*Select a alcohol to see cocktails.*",
                                        ParseMode.Markdown,
                                        replyToMessageId: e.Message.MessageId,
                                        replyMarkup: selections);
                                }
                                else if (DatabaseHandler.DoesExistInCocktails(message))
                                {
                                    botAnswer = DatabaseHandler.GetCocktailRecipe(message);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                }
                                else if (DatabaseHandler.DoesExistInCocktailTypes(message))
                                {
                                    var cocktailNames = DatabaseHandler.GetCocktailNames(message);
                                    cocktailNames.Add("Go Back");
                                    var buttons = cocktailNames.Select(cocktailName => new[] { new KeyboardButton(cocktailName) })
                                        .ToArray();
                                    var selections = new ReplyKeyboardMarkup(buttons);

                                    await botClient.SendTextMessageAsync(e.Message.Chat,
                                        "*Select a cocktail to see its recipe.*",
                                        ParseMode.Markdown,
                                        replyToMessageId: e.Message.MessageId,
                                        replyMarkup: selections);
                                }
                                else if (DatabaseHandler.DoesUserHaveThisCommand(message, username))
                                {
                                    Command command = DatabaseHandler.GetCommandFrom(message, username);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, command.Reply, ParseMode.Markdown);
                                }
                                else if (DatabaseHandler.DoesExistInLocations(message, username))
                                {
                                    Location location = DatabaseHandler.GetLocation(message, username);
                                    if (location.WhoAdded == currentUser.Username)
                                    {
                                        float latitude = location.Latitude;
                                        float longitude = location.Longitude;

                                        botAnswer = message + " konum";
                                        await botClient.SendVenueAsync(e.Message.Chat,
                                            latitude,
                                            longitude,
                                            message,
                                            "");
                                    }
                                }
                                else if (DatabaseHandler.DoesExistInReminders(message, username))
                                {
                                    Reminder reminder = DatabaseHandler.GetReminder(message, username);
                                    if (reminder.WhoAdded == currentUser.Username)
                                    {
                                        botAnswer = string.Format(
                                            "*Name:* {0}\n" +
                                            "*Message:* {1}\n" +
                                            "*Time:* {2}"
                                            , reminder.Name, reminder.Text, reminder.Date);

                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                }
                            }
                        }
                        else
                        {
                            botAnswer = "*To start using the bot, please write \"/start\" .*";
                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                        }
                    }
                    else
                    {
                        botAnswer = "*To use the bot, you should set an username on Telegram.*";
                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                    }
                    if (botAnswer.Length > 1)
                    {
                        log.Message = botAnswer;
                        log.Sender = botName;
                        log.LogName = username;
                        LogHandler.Log(log);
                    }
                    if (announce.Length > 1)
                    {
                        log.Message = announce;
                        log.Sender = botName;
                        log.LogName = "ANNOUNCES";
                        LogHandler.Log(log);
                    }
                }
            }
        }
        private static async Task<bool> SendMessage(string announce, string toWho)
        {
            try
            {
                string chatId = DatabaseHandler.GetUser(toWho).ChatId;
                await botClient.SendTextMessageAsync(chatId, announce, ParseMode.Markdown);
                return true;
            }
            catch (Exception ex)
            {
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        private static async Task<bool> BruteAnnounce(string announce, string toWho, int howMany)
        {
            try
            {
                string chatId = DatabaseHandler.GetUser(toWho).ChatId;
                for (int i = 0; i < howMany; i++)
                {
                    await botClient.SendTextMessageAsync(chatId, announce, ParseMode.Markdown);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHandler.ReportCrash(ex);
                return false;
            }
        }

        private static string GetCommands(string username)
        {
            string botAnswer = "*There are " + DatabaseHandler.CountCommandsFrom(username) + " commands:*";
            List<Command> userCommands = DatabaseHandler.GetCommandsFrom(username);

            foreach (var command in userCommands)
            {
                botAnswer += "\n" + command.Question;
            }
            return botAnswer;
        }

        internal void Disconnect()
        {
            botClient.StopReceiving();
        }
    }
}