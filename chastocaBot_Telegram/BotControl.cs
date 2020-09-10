using System;
using System.Collections.Generic;
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
            string answer = "";
            string message = e.Message.Text;
            string[] fragmentedMessage = message.Split(' ', ':', ';', '\t');
            string username = e.Message.From.Username;
            string chatId = e.Message.Chat.Id.ToString();
            string botName = "chastocaBot";


            if (message != null)
            {
                LogHandler.Log(message, username, username);
                if (message.Equals("/start"))
                {
                    DatabaseHandler.AddUser(username, chatId);
                    await botClient.SendTextMessageAsync(e.Message.Chat, DatabaseHandler.GetReply("!help"), ParseMode.Markdown);
                }
                else if (DatabaseHandler.DoesExistInUsers(username, chatId))
                {
                    if (!message.Contains("'"))
                    {
                        if (message.StartsWith("!feedback"))
                        {
                            if (message.Length > 12)
                            {
                                string feedback = message.Substring(10);
                                LogHandler.Log(feedback, username, "FEEDBACKS");
                                answer = "Thank you for your feedback!";
                                await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                            }
                        }
                        else if (message.Equals("loc") && username.Equals("ardaerbaharli"))
                        {
                            var locations = DatabaseHandler.GetLocationNames();
                            var buttons = locations.Select(location => new[] { new KeyboardButton(location) })
                                .ToArray();
                            var selections = new ReplyKeyboardMarkup(buttons);

                            await botClient.SendTextMessageAsync(e.Message.Chat,
                                "*Konum seç.*",
                                ParseMode.Markdown,
                                replyToMessageId: e.Message.MessageId,
                                replyMarkup: selections);
                        }
                        else if (message.StartsWith("!addlocation") && username.Equals("ardaerbaharli"))  //!addlocation arda lati longi
                        {
                            if (fragmentedMessage.Length == 4)
                            {
                                string name = fragmentedMessage[1];
                                int startOffset = name.Length + fragmentedMessage[0].Length + 2;
                                float latitude = float.Parse(fragmentedMessage[2], CultureInfo.InvariantCulture.NumberFormat);
                                float longitude = float.Parse(fragmentedMessage[3], CultureInfo.InvariantCulture.NumberFormat);


                                bool isSuccessful = DatabaseHandler.AddLocation(name, latitude, longitude);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Location added: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!addlocation <location_name> <latitude> <longitude>", ParseMode.Markdown);
                        }
                        else if (message.StartsWith("!deletelocation") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length == 2)
                            {
                                string name = fragmentedMessage[1];
                                bool isSuccessful = DatabaseHandler.DeleteLocation(name);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Location deleted: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!deletelocation <name>", ParseMode.Markdown);                            
                        }
                        else if (message.StartsWith("!addcommand") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length > 2)
                            {
                                string command = fragmentedMessage[1];
                                int startOffset = command.Length + fragmentedMessage[0].Length + 2;
                                string reply = message[startOffset..message.Length].Trim();
                                bool isSuccessful = DatabaseHandler.AddCommand(command, reply);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Command added: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!addcommand <command> <reply>", ParseMode.Markdown);
                        }
                        else if (message.StartsWith("!deletecommand") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length == 2)
                            {
                                string command = fragmentedMessage[1];
                                bool isSuccessful = DatabaseHandler.DeleteCommand(command);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Command deleted: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecommand <command>", ParseMode.Markdown);
                        }
                        else if (message.StartsWith("!updatecommand") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length > 2)
                            {
                                string command = fragmentedMessage[1];
                                int startOffset = command.Length + fragmentedMessage[0].Length + 2;
                                string reply = message[startOffset..message.Length].Trim();
                                bool isSuccessful = DatabaseHandler.ChangeCommand(command, reply);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Command updated: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!changecommand <command> <newReply>", ParseMode.Markdown);
                        }
                        else if (message.StartsWith("!addcocktail") && username.Equals("ardaerbaharli")) //!addcocktail name type recipe
                        {
                            if (fragmentedMessage.Length > 3)
                            {
                                string cocktailName = fragmentedMessage[1];
                                string cocktailType = fragmentedMessage[2];
                                int startOffset = cocktailName.Length + cocktailType.Length + fragmentedMessage[0].Length + 3;
                                string recipe = message[startOffset..message.Length].Trim();

                                DatabaseHandler.AddCocktailType(cocktailType);
                                bool isSuccessful = DatabaseHandler.AddCocktail(cocktailName, recipe, cocktailType);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Cocktail added: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!addcocktail <cocktailName> <cocktailtype> <recipe>", ParseMode.Markdown);                            
                        }
                        else if (message.StartsWith("!deletecocktail") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length == 2)
                            {
                                string cocktailName = fragmentedMessage[1];
                                bool isSuccessful = DatabaseHandler.DeleteCocktail(cocktailName);
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Cocktail deleted: " + isSuccessful, ParseMode.Markdown);
                            }
                            else                            
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!deletecocktail <cocktailName>", ParseMode.Markdown);
                        }
                        else if (message.StartsWith("!announce") && username.Equals("ardaerbaharli"))
                        {
                            if (fragmentedMessage.Length > 1)
                            {

                                int startOffset = fragmentedMessage[0].Length + 1;
                                string announce = message[startOffset..message.Length].Trim();
                                bool isSuccessful = await Announce(announce);

                                await botClient.SendTextMessageAsync(e.Message.Chat, "Announce successfull: " + isSuccessful, ParseMode.Markdown);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(e.Message.Chat, "!announce <announce>", ParseMode.Markdown);

                            }
                        }
                        else if (message.Equals("!win") && username != null)
                        {
                            string counter = DatabaseHandler.AddCounter("!win", username);
                            answer = "Win count of " + username + ": " + counter;
                            await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                        }
                        else if (message.Equals("!wincount") && username != null)
                        {
                            string counter = DatabaseHandler.FindCounterAnswer("!win", username);
                            answer = "Win count of " + username + ": " + counter;
                            await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                        }
                        else if (message.Equals("!leaderboard"))
                        {
                            answer = "*---Leaderboard---*\n";
                            string[,] leaderboard = DatabaseHandler.GetLeaderboard("!win");
                            for (int i = 0; i < 5; i++)
                            {
                                if (leaderboard[i, 0] != null)
                                {
                                    answer += "*" + leaderboard[i, 0] + "* : " + leaderboard[i, 1];
                                    answer += "\n";
                                }
                            }
                            await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                        }
                        else if (message.Equals("!cocktails"))
                        {
                            var cocktailTypes = DatabaseHandler.GetCocktailTypes();
                            cocktailTypes.Add("Go Back");
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
                        else if (DatabaseHandler.DoesExistInCocktails(message))
                        {
                            answer = DatabaseHandler.GetCocktailRecipe(message);
                            await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                        }
                        else if (DatabaseHandler.DoesExistInCommands(message))
                        {
                            answer = DatabaseHandler.GetReply(message);
                            await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                        }
                        else if (DatabaseHandler.DoesExistInCocktailTypes(message))
                        {
                            var cocktailNames = DatabaseHandler.GetCocktailNames(message);
                            var buttons = cocktailNames.Select(cocktailName => new[] { new KeyboardButton(cocktailName) })
                                .ToArray();
                            var selections = new ReplyKeyboardMarkup(buttons);

                            await botClient.SendTextMessageAsync(e.Message.Chat,
                                "*Select a cocktail to see its recipe.*",
                                ParseMode.Markdown,
                                replyToMessageId: e.Message.MessageId,
                                replyMarkup: selections);
                        }
                        else if (DatabaseHandler.DoesExistInLocations(message) && username.Equals("ardaerbaharli"))
                        {
                            var location = DatabaseHandler.GetLocation(message);
                            CultureInfo numberFormat = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            numberFormat.NumberFormat.CurrencyDecimalSeparator = ".";
                            float latitude = float.Parse(location.Latitude, NumberStyles.Any, numberFormat);
                            float longitude = float.Parse(location.Longitude, NumberStyles.Any, numberFormat);

                            answer = message + " konum";
                            await botClient.SendVenueAsync(e.Message.Chat,
                                latitude,
                                longitude,
                                message,
                                "");
                        }
                    }
                }
                else
                {
                    answer = "To start using the bot, please write \"/start\" .";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);

                }
                if (answer.Length > 1)
                    LogHandler.Log(answer, botName, username);


            }
        }
        private static async Task<bool> Announce(string announce)
        {
            try
            {
                List<User> Users = DatabaseHandler.GetUsers();
                foreach (var user in Users)
                {
                    await botClient.SendTextMessageAsync(user.ChatId, announce, ParseMode.Markdown);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        internal void Disconnect()
        {
            botClient.StopReceiving();
        }
    }
}