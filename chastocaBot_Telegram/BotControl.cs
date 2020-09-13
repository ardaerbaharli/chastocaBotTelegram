using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
        private enum Ranks { Folk = 1, Berserker, Gatekeeper, King }
        internal void Connect()
        {
            botClient = new TelegramBotClient(botToken);
            var bot = botClient.GetMeAsync().Result;
            Console.WriteLine(bot.Username + " is online!");

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            string botAnswer = "";
            string botAnswer2 = "";
            string message = e.Message.Text;
            string[] fragmentedMessage = message.Split(' ', ':', ';', '\t');
            string username = e.Message.From.Username;
            string chatId = e.Message.Chat.Id.ToString();
            string botName = "chastocaBot";
            string startingRank = "Folk";
            string announce = "";
            if (message != null)
            {
                Log log = new Log
                {
                    Message = message,
                    Sender = username,
                    LogName = username
                };
                LogHandler.Log(log);
                if (username != null)
                {
                    if (message.Equals("/start"))
                    {
                        User newUser = new User
                        {
                            Username = username,
                            ChatId = chatId,
                            Rank = startingRank
                        };
                        DatabaseHandler.AddUser(newUser);
                        botAnswer = Commands(startingRank);
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
                            else if (message.Equals("!mylocations") && CanAccess(currentUser, "Berserker"))
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
                            else if (fragmentedMessage[0].Equals("!addlocation") && CanAccess(currentUser, "Berserker"))  //!addlocation arda lati longi
                            {
                                if (fragmentedMessage.Length == 4)
                                {
                                    if (currentUser.Rank == "Berserker")
                                    {
                                        if (DatabaseHandler.CountLocationsFrom(username) < 6)
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
                                    }
                                    else
                                    {
                                        Location location = new Location
                                        {
                                            WhoAdded = currentUser.Username,
                                            Name = fragmentedMessage[1],
                                            Latitude = float.Parse(fragmentedMessage[2], CultureInfo.InvariantCulture.NumberFormat),
                                            Longitude = float.Parse(fragmentedMessage[3], CultureInfo.InvariantCulture.NumberFormat)
                                        };
                                        bool isSuccessful = DatabaseHandler.AddLocation(location);
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "Location added: " + isSuccessful, ParseMode.Markdown);
                                    }
                                }
                                else
                                {
                                    botAnswer2 = "!addlocation <locationName> <latitude> <longitude>\nLatitude and longitude values should be like: 39.895613 32.798303";
                                    char x = botAnswer2.ElementAt(22);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer2, ParseMode.Markdown);
                                }
                            }
                            else if (fragmentedMessage[0].Equals("!deletelocation") && CanAccess(currentUser, "Berserker"))
                            {
                                if (fragmentedMessage.Length == 2)
                                {
                                    string name = fragmentedMessage[1];
                                    if (IsAdmin(currentUser))
                                    {
                                        bool isSuccessful = DatabaseHandler.DeleteLocation(name);
                                        botAnswer = "Location deleted: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                    {
                                        Location location = DatabaseHandler.GetLocation(name);
                                        if (location.WhoAdded == username)
                                        {
                                            bool isSuccessful = DatabaseHandler.DeleteLocation(name);
                                            botAnswer = "Location deleted: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                    }

                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!deletelocation <name>", ParseMode.Markdown);
                            }
                            else if (message.Equals("!mycommands") && CanAccess(currentUser, "Berserker"))
                            {
                                botAnswer = "You have " + DatabaseHandler.CountCommandsFrom(username) + " commands:";
                                List<Command> userCommands = DatabaseHandler.GetCommandsFrom(username);

                                foreach (var command in userCommands)
                                {
                                    botAnswer += "\n" + command.Question;
                                }
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!addcommand") && CanAccess(currentUser, "Berserker"))
                            {

                                if (IsAdmin(currentUser))
                                {
                                    if (fragmentedMessage.Length > 3)
                                    {
                                        Command command = new Command();

                                        command.WhoCanAccess = fragmentedMessage[1];
                                        command.Question = fragmentedMessage[2];
                                        command.WhoAdded = username;
                                        int startOffset = command.Question.Length + command.WhoCanAccess.Length + fragmentedMessage[0].Length + 3; // 3f or spaces
                                        command.Reply = message[startOffset..message.Length].Trim();

                                        bool isSuccessful = DatabaseHandler.AddCommand(command);
                                        botAnswer = "Command added: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!addcommand <whoCanAccess> <command> <reply>", ParseMode.Markdown);
                                }
                                else
                                {
                                    if (fragmentedMessage.Length > 2)
                                    {
                                        if (DatabaseHandler.CountCommandsFrom(username) < 6)
                                        {
                                            Command command = new Command();

                                            command.WhoCanAccess = username;
                                            command.Question = fragmentedMessage[1];
                                            command.WhoAdded = username;
                                            int startOffset = command.Question.Length + fragmentedMessage[0].Length + 2; // 2 for spaces
                                            command.Reply = message[startOffset..message.Length].Trim();

                                            bool isSuccessful = DatabaseHandler.AddCommand(command);
                                            botAnswer = "Command added: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                        else
                                        {
                                            botAnswer = "You've reached the limit of 5 custom commands. You can update or delete your commands with !updatecommand and !deletecommand.";
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "!addcommand <command> <reply>", ParseMode.Markdown);
                                }
                            }
                            else if (fragmentedMessage[0].Equals("!deletecommand") && CanAccess(currentUser, "Berserker"))
                            {
                                if (fragmentedMessage.Length == 2)
                                {
                                    string question = fragmentedMessage[1];
                                    if (IsAdmin(currentUser))
                                    {
                                        bool isSuccessful = DatabaseHandler.DeleteCommand(question);
                                        botAnswer = "Command deleted: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                    {
                                        Command command = DatabaseHandler.GetCommand(question);
                                        if (command.WhoAdded == currentUser.Username)
                                        {
                                            bool isSuccessful = DatabaseHandler.DeleteCommand(question);
                                            botAnswer = "Command deleted: " + isSuccessful;
                                            await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                        }
                                    }
                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecommand <command>", ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!updatecommand") && CanAccess(currentUser, "Berserker"))
                            {
                                if (fragmentedMessage.Length > 2)
                                {
                                    Command command = new Command();
                                    command.Question = fragmentedMessage[1];
                                    command.WhoCanAccess = fragmentedMessage[2];
                                    int startOffset = command.Question.Length + command.WhoCanAccess.Length + fragmentedMessage[0].Length + 3; //3 for spaces
                                    command.Reply = message[startOffset..message.Length].Trim();
                                    Command oldCommand = DatabaseHandler.GetCommand(command.Question);
                                    if (oldCommand.WhoAdded == currentUser.Username || IsAdmin(currentUser))
                                    {
                                        bool isSuccessful = DatabaseHandler.UpdateCommand(command);
                                        botAnswer = "Command updated: " + isSuccessful;
                                        await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "You don't have permission to change this command.", ParseMode.Markdown);
                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!updatecommand <command> <whoCanAccess> <newReply>", ParseMode.Markdown);

                            }
                            else if (fragmentedMessage[0].Equals("!deletecommandsfrom") && CanAccess(currentUser, "Gatekeeper"))
                            {
                                if (fragmentedMessage.Length == 2)
                                {
                                    string question = fragmentedMessage[1];
                                    bool isSuccessful = DatabaseHandler.DeleteCommand(question);
                                    botAnswer = "Commands deleted: " + isSuccessful;
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecommandsfrom <username>", ParseMode.Markdown);
                            }
                            else if (message.Equals("!commands") && CanAccess(currentUser, startingRank))
                            {
                                botAnswer = Commands(startingRank);
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (message.Equals("!admincommands") && CanAccess(currentUser, "Gatekeeper"))
                            {
                                botAnswer = Commands("Gatekeeper");
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!addcocktail") && CanAccess(currentUser, "Gatekeeper")) //!addcocktail name type recipe
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
                            else if (fragmentedMessage[0].Equals("!deletecocktail") && CanAccess(currentUser, "Gatekeeper"))
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
                            else if (fragmentedMessage[0].Equals("!requestcocktail") && CanAccess(currentUser, startingRank))
                            {
                                if (fragmentedMessage.Length == 2)
                                {
                                    string cocktailName = fragmentedMessage[1];
                                    Log cocktailLog = new Log();
                                    log.LogName = "COCKTAIL_REQUESTS";
                                    log.Message = cocktailName;
                                    log.Sender = username;
                                    LogHandler.Log(cocktailLog);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "We will add the requested cocktail as soon as possible.", ParseMode.Markdown);
                                    announce = string.Format("{0} requested {1} named cocktail to be added to the cocktails.", username, cocktailName);
                                    await Announce(announce, "King");
                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!requestcocktal <cocktailName>", ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!giverank") && CanAccess(currentUser, "Gatekeeper"))
                            {
                                if (fragmentedMessage.Length == 3)
                                {
                                    User user = new User();
                                    user.Username = fragmentedMessage[1];
                                    user.Rank = fragmentedMessage[2];
                                    user.ChatId = DatabaseHandler.GetUser(user.Username).ChatId;

                                    bool isSuccessful = DatabaseHandler.UpdateUser(user);
                                    botAnswer = string.Format("{0} named user updated {1} and new rank is {2} ", user.Username, isSuccessful, user.Rank);
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                    await botClient.SendTextMessageAsync(user.ChatId, string.Format("Your new rank is: *{0}*", user.Rank), ParseMode.Markdown);
                                }
                                else
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!giverank <username> <rank>", ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!listusers") && CanAccess(currentUser, "Gatekeeper"))
                            {
                                List<User> Users = DatabaseHandler.GetUsers();
                                botAnswer = "*--Username--*      *--Rank--*";
                                foreach (var user in Users)
                                    botAnswer += string.Format("\n{0}     {1}", user.Username, user.Rank);
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (fragmentedMessage[0].Equals("!announce") && CanAccess(currentUser, "Gatekeeper"))
                            {
                                if (fragmentedMessage.Length > 2)
                                {
                                    string ranksToAnnounce = fragmentedMessage[1];
                                    int startOffset = fragmentedMessage[0].Length + fragmentedMessage[1].Length + 1;
                                    announce = message[startOffset..message.Length].Trim();
                                    bool isSuccessful = await Announce(announce, ranksToAnnounce);
                                    botAnswer = "Announce successfull: " + isSuccessful;
                                    await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "!announce <ranksToAnnounce> <announce> \n(Folk = All,Berserker = Except folk, Gatekeeper = Except berserker and folk)", ParseMode.Markdown);
                                }
                            }
                            else if (message.Equals("!win") && CanAccess(currentUser, startingRank))
                            {
                                string counter = DatabaseHandler.AddCounter("!win", username);
                                botAnswer = "Win count of " + username + ": " + counter;
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (message.Equals("!wincount") && CanAccess(currentUser, startingRank))
                            {
                                string counter = DatabaseHandler.FindCounterAnswer("!win", username);
                                botAnswer = "Win count of " + username + ": " + counter;
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (message.Equals("!leaderboard") && CanAccess(currentUser, startingRank))
                            {
                                botAnswer = "*---Leaderboard---*\n";
                                string[,] leaderboard = DatabaseHandler.GetLeaderboard("!win");
                                for (int i = 0; i < 5; i++)
                                {
                                    if (leaderboard[i, 0] != null)
                                    {
                                        botAnswer += "*" + leaderboard[i, 0] + "* : " + leaderboard[i, 1];
                                        botAnswer += "\n";
                                    }
                                }
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (message.Equals("!cocktails") && CanAccess(currentUser, startingRank))
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
                            else if (DatabaseHandler.DoesExistInCocktails(message) && CanAccess(currentUser, startingRank))
                            {
                                botAnswer = DatabaseHandler.GetCocktailRecipe(message);
                                await botClient.SendTextMessageAsync(e.Message.Chat, botAnswer, ParseMode.Markdown);
                            }
                            else if (DatabaseHandler.DoesExistInCocktailTypes(message) && CanAccess(currentUser, startingRank))
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
                            else if (DatabaseHandler.DoesExistInCommands(message) && CanAccess(currentUser, startingRank))
                            {
                                Command command = DatabaseHandler.GetCommand(message);
                                if (CanAccess(currentUser, command.WhoCanAccess))
                                    await botClient.SendTextMessageAsync(e.Message.Chat, command.Reply, ParseMode.Markdown);
                            }
                            else if (DatabaseHandler.DoesExistInLocations(message) && CanAccess(currentUser, "Berserker"))
                            {
                                Location location = DatabaseHandler.GetLocation(message);
                                if (CanAccess(currentUser, location.WhoAdded))
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
        private static async Task<bool> Announce(string announce, string ranksToAnnounce)
        {
            try
            {
                List<User> Users = DatabaseHandler.GetUsers();
                switch (ranksToAnnounce)
                {
                    case "Folk":
                        foreach (var user in Users)
                        {
                            await botClient.SendTextMessageAsync(user.ChatId, announce, ParseMode.Markdown);
                        }
                        break;
                    case "Berserker":
                        foreach (var user in Users)
                        {
                            if (user.Rank != "Folk")
                                await botClient.SendTextMessageAsync(user.ChatId, announce, ParseMode.Markdown);
                        }
                        break;
                    case "Gatekeeper":
                        foreach (var user in Users)
                        {
                            if (user.Rank.Equals("Gatekeeper") || user.Rank.Equals("King"))
                                await botClient.SendTextMessageAsync(user.ChatId, announce, ParseMode.Markdown);
                        }
                        break;
                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        private static bool CanAccess(User currentUser, string whoCanAccess)
        {
            bool canAccess = false;
            bool isDefinedInRanksUserRank = Enum.IsDefined(typeof(Ranks), currentUser.Rank);
            bool isDefinedInRanksWhoCanAccess = Enum.IsDefined(typeof(Ranks), whoCanAccess);

            if (isDefinedInRanksUserRank && isDefinedInRanksWhoCanAccess)
            {
                Ranks rankEnum = (Ranks)Enum.Parse(typeof(Ranks), currentUser.Rank);
                Ranks whoCanAccessEnum = (Ranks)Enum.Parse(typeof(Ranks), whoCanAccess);
                if (rankEnum >= whoCanAccessEnum)
                    canAccess = true;
            }
            else if (currentUser.Username == whoCanAccess)//|| IsAdmin(currentUser)
                canAccess = true;

            return canAccess;
        }
        private static string Commands(string rank)
        {
            string botAnswer = "*There are " + DatabaseHandler.CountCommandsWhoCanAccess(rank) + " commands:*";
            List<Command> userCommands = DatabaseHandler.GetCommandsWhoCanAccess(rank);

            foreach (var command in userCommands)
            {
                botAnswer += "\n" + command.Question;
                switch (command.Question)
                {
                    case "!win":
                        botAnswer += " >>> You can join Fall Guys leaderboard.";
                        break;
                    case "!wincount":
                        botAnswer += " >>> Your Fall Guys win count.";
                        break;
                    case "!leaderboard":
                        botAnswer += " >>> Fall Guys leaderboard.";
                        break;
                    default:
                        break;
                }

            }
            return botAnswer;
        }
        private static bool IsAdmin(User currentuser)
        {
            bool isAdmin = false;
            if (currentuser.Rank == Ranks.King.ToString() || currentuser.Rank == Ranks.Gatekeeper.ToString())
                isAdmin = true;

            return isAdmin;
        }
        internal void Disconnect()
        {
            botClient.StopReceiving();
        }
    }
}