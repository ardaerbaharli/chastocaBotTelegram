using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace chastocaBot_Telegram
{
    class BotControl
    {
        private static string botToken = "1280937266:AAHK0ETudaO0yPXttoEAiCoLn6Ypgc0dyZg";
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

            string answer = "", counter;
            string message = e.Message.Text;
            string username = e.Message.From.Username;
            string bot = "chastocaBot";

            if (e.Message.Text != null)
            {
                string rules = "*To use the bot you should set an username! *" +
                    "\n\n *Commands* " +
                    "\n\n !win" +
                    "\n !wincount" +
                    "\n !leaderboard" +
                    "\n !pu";

                LogHandler.Log(message, username);

                if (message.Equals("/start"))
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, rules, ParseMode.Markdown);
                }
                else if (message.Equals("!pu"))
                {
                    answer = "*pu*";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!win"))
                {
                    counter = CommandHandler.AddCounter("!win", username);
                    answer = "Win count of " + username + ": " + counter;
                    LogHandler.Log(answer, bot);
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!wincount"))
                {
                    counter = CommandHandler.FindCounterAnswer("!win", username);
                    answer = "Win count of " + username + ": " + counter;
                    LogHandler.Log(answer, bot);
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!leaderboard"))
                {
                    answer = "*---Leaderboard---*\n";
                    string[,] leaderboard = CommandHandler.GetLeaderboard("!win");
                    LogHandler.Log("--- LEADERBOARD ---", bot);
                    for (int i = 0; i < 5; i++)
                    {
                        if (leaderboard[i, 0] != null)
                        {
                            answer += "*" + leaderboard[i, 0] + "* : " + leaderboard[i, 1];
                            answer += "\n";
                        }
                    }
                    LogHandler.Log(answer, bot);
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!cocktails"))
                {
                    var categories = new[] { "Gin", "Vodka", "Tequila", "Rum", "Whiskey", "Mix", "Syrups", "Alcohol free" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a alcohol to see cocktails.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Gin"))
                {
                    var categories = new[] { "Basil Smash", "Bees Kness", "Bramble", "Clover Club", "Fresh 75", "Gin Fizz", "Tom collins", "Cosmo x Breakfast Martini" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a cocktail to see its recipe.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Basil Smash"))
                {
                    answer = "*--BASIL SMASH--*";
                    answer += "\nIn the mid 2000s, a German bartender create the Basil Smash - a herbaceous gin sour recipe.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Gin (2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    answer += "\n - 2-3 Springs of fresh basil";
                    answer += "\n-- You can use to garnish.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Bees Kness"))
                {
                    answer = "*--Bees Kness--*";
                    answer += "\nThe Bee's Knees cocktail recipe was invented out of necessity, hiding the poor quality homemade, bathhub cut gin that was prevalent during the prohibition era.";
                    answer += "\n\n *INGREDIENTS*";
                    answer += "\n - 60ml Gin (2 oz)";
                    answer += "\n - 15ml Fresh lemon juice (1/2 oz)";
                    answer += "\n - 15ml Honey syrup (1/2 oz)";
                    answer += "\n-- You can use twist of lemon or wheel to garnish.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Bramble"))
                {
                    answer = "*--Bramble--*";
                    answer += "\nIn 1984, London Bartender, tweaked a classic gin sour by adding Creme de Mure. Use crushed ice so that the Creme de Mure cascades down making a nice visual effect whilst adding a great balance of sweet, tart and fruitiness..";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Gin (2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    answer += "\n - 2-3 Springs of fresh basil";
                    answer += "\n-- You can use to garnish.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Clover Club"))
                {
                    answer = "*--Clover Club--*";
                    answer += "\nA pre-prohibition cocktail that was named after a Philadelphia men's group consisting of business professionals. First published in 1901 in the New York Press. It fell out of fashion by the 1950s and became more of a ladies drink.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 45ml Gin (3/2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Raspberry syrup (3/4 oz)";
                    answer += "\n - Fresh egg white (half a small egg)";
                    answer += "\n-- Traditionally served with no garnish but you can utilise a lemon twist or skewered raspberry if you like.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Fresh 75"))
                {
                    answer = "*--Fresh 75--*";
                    answer += "\nThe original version of the French 75 calls for cognac - but gin is the more common choice nowadays. It has a similar framework to a collins with the substitution of champagne or sparkling wine instead of soda.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 30ml Gin (1 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 30ml 1:1 Simple syrup (1 oz)";
                    answer += "\n - 90ml Champagne or other sparkling wine (3 oz)";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Gin Fizz"))
                {
                    answer = "*--Gin Fizz--*";
                    answer += "\nA Fizz is a sour lengthened with soda water. It differs from a collins as it's shaken and has the addition of egg white. You can omit egg white if you prefer not use it in your drinks.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Gin (2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4  oz)";
                    answer += "\n - Fresh egg white (half a small egg)";
                    answer += "\n - Soda water";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Tom collins"))
                {
                    answer = "*--Gin Fizz--*";
                    answer += "\nThe Tom Collins would be one of the most well known of the Collins', made using a sweeter style, Old Tom gin. The collins cocktail is a great base to create variations by adding complimantery components and flavours such as elderflower, lychee and other fruit flavours in the form of fresh fruit, syrups and liqueurs. ";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Preferably Old Tom Gin (2 oz) ";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 15ml Simple syrup (1/2  oz)";
                    answer += "\n - Dash of Soda";
                    answer += "\n - Some Collins' recipes call for a dash or two of bitters but i usually omit this.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Cosmo x Breakfast Martini"))
                {
                    answer = "*--Cosmo x Breakfast Martini--*";
                    answer += "\nThe Cosmonaut was created by Sasha Petraske, founder of NY bar Milk & Honey. A simple 3 ingredient cocktail that is a sly retort to the well known Cosmopolitan. It has a resemblance to the 1934 Cosmopolitan and is a slight variation on the Breakfast Martini (raspberry jam instead of marmalade).";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Dry Gin (2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 2 Barspoons of raspberry jam";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Vodka"))
                {
                    var categories = new[] { "Caipiroska", "Chocolote Martini", "Lemon Drop Martini", "Espresso Martini", "Moscow Mule" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a cocktail to see its recipe.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Caipiroska"))
                {
                    answer = "*--Caipiroska--*";
                    answer += "\nThe Caipiroska is an easy drink to create. Nail the balance of sweet and sour flavours, load your glass with crushed ice and you’ll keep coming back for more.";
                    answer += "\n\n *INGREDIENTS*";
                    answer += "\n - 60ml Vodka (2 oz)";
                    answer += "\n - 1/2 Lime";
                    answer += "\n - 2 Heaped teaspoons of sugar (preferably brown)";
                    answer += "\n-- Garnish with a lime wheel or wedge";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Chocolote Martini"))
                {
                    answer = "*--Bramble--*";
                    answer += "\nThe easiest Chocolate Martini recipe you can find! A simple two ingredient recipe consisting of vodka and creme de cacao. You can use brown or white creme de cacao but the white retains the clarity of a classic Martini.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Vodka (2 oz)";
                    answer += "\n - 45ml Creme de Cacao (1.5 oz)";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Lemon Drop Martini"))
                {
                    answer = "*--Lemon Drop Martini--*";
                    answer += "\nThe Lemon Drop Martini was created back in the 80s during the vodka craze. It was apparently named after a hard candy and said to be made in the bay area of San Francisco.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Gin (2 oz)";
                    answer += "\n - 7.5ml Cointreau (1/4 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Espresso Martini"))
                {
                    answer = "*--Espresso Martini--*";
                    answer += "\nThe Espresso Martini is an Australian favourite. Rich, strong coffee notes and velvety smooth. Dick Bradsell first created the Espresso Martini in the 80s - he called for Wyborowa vodka and Kahlua as his choice of coffee liqueur.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Vodka (2 oz)";
                    answer += "\n - 30ml Espresso (1 oz)";
                    answer += "\n - 15ml Coffee liqueur (1/2 oz)";
                    answer += "\n - 7.5ml 1:1 Simple syrup (3/4 oz)";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Moscow Mule"))
                {
                    answer = "*--Moscow Mule--*";
                    answer += "\nThe drink was named the Moscow Mule due to the 1940s American perception that vodka was a Russian spirit and its intense ginger kick (like a Mule). It was originally made with Smirnoff vodka..";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Vodka (2 oz)";
                    answer += "\n - 90ml Ginger Beer (3 oz)";
                    answer += "\n - 15ml Fresh lime juice (1/2 oz)";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Tequila"))
                {
                    var categories = new[] { "Margarita" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a cocktail to see its recipe.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Margarita"))
                {
                    answer = "*--Margarita--*";
                    answer += "\nAccording to cocktail historian David Wondrich, the margarita is merely a popular Mexican and American drink, the Daisy (margarita is Spanish for “daisy”), remade with tequila instead of brandy, which became popular during Prohibition as people drifted over the border for alcohol. There is an account from 1936 of Iowa newspaper editor James Graham finding such a cocktail in Tijuana, years before any of the other margarita “creation myths”.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - ";
                    answer += "\n - ";
                    answer += "\n - ";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Rum"))
                {

                    var categories = new[] { "Mojito" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a cocktail to see its recipe.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Mojito"))
                {
                    answer = "*--Mojito--*";
                    answer += "\nThe Mojito cocktail is the quintessential classic Cuban cocktail. A simply constructed drink with fresh ingredients. White rum, fresh lime juice, sugar and fresh mint makes for a delicious and refreshing Mojito.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml White Rum (2 oz)";
                    answer += "\n - 90ml Fresh lime juice (3/4 oz)";
                    answer += "\n - 15ml Sugar syrup (3/4 oz)";
                    answer += "\n - 60ml Soda water (2 oz)";
                    answer += "\n - 6-8 Mint leaves";
                    answer += "\n - Don't strain and garnish with a spring of fresh mint";

                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Whiskey")) { }
                else if (message.Equals("Mix")) { }
                else if (message.Equals("Syrups")) { }
                else if (message.Equals("Alcohol free")) { }


            }
        }

        internal void Disconnect()
        {
            botClient.StopReceiving();
        }
    }
}