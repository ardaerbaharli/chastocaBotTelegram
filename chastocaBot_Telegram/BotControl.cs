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
            string commands = "\n\n *Commands* " +
                   "\n\n !feedback <Tell me how to improve the bot!>" +
                   "\n !help" +
                   "\n !commands" +
                   "\n !win" +
                   "\n !wincount" +
                   "\n !leaderboard" +
                   "\n !pu" +
                   "\n !woo" +
                   "\n !cocktails";
            string rules = "*To use some features of the bot you should set an username! *" + commands;

            if (message != null)
            {

                LogHandler.Log(message, username);

                if (message.Equals("/start"))
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, rules, ParseMode.Markdown);
                }
                else if (message.Equals("!feedback"))
                {
                    string feedback = "";
                    feedback += "username: ";
                    feedback += message.Substring(10);
                    LogHandler.Log(feedback, "FEEDBACKS");
                    answer = "Thank you for your feedback!";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!help"))
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, rules, ParseMode.Markdown);
                }
                else if (message.Equals("!commands"))
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, commands, ParseMode.Markdown);
                }
                else if (message.Equals("!woo"))
                {
                    answer = "*WOHOO*";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!pu"))
                {
                    answer = "*pu*";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!win") && username != null)
                {
                    counter = CommandHandler.AddCounter("!win", username);
                    answer = "Win count of " + username + ": " + counter;
                    LogHandler.Log(answer, bot);
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("!wincount") && username != null)
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
                    var categories = new[] { "Gin", "Vodka", "Tequila", "Rum", "Whiskey", "Mix", "Syrups", "Alcohol free", "Shots" };
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
                    var categories = new[] { "Basil Smash", "Bees Kness", "Bramble", "Clover Club", "Fresh 75", "Gin Fizz", "Tom collins", "Cosmo x Breakfast Martini", "Go Back" };
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
                    answer += "\n\n - Garnish with a spring of basil.";
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
                    answer += "\n\n - You can use twist of lemon or wheel to garnish.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Bramble"))
                {
                    answer = "*--Bramble--*";
                    answer += "\nIn 1984, London Bartender, tweaked a classic gin sour by adding Creme de Mure. Use crushed ice so that the Creme de Mure cascades down making a nice visual effect whilst adding a great balance of sweet, tart and fruitiness..";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 45ml Gin (3/2 oz)";
                    answer += "\n - 22.5ml Fresh lemon juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    answer += "\n - 22.5ml Creme de Mure (3/4 oz)";
                    answer += "\n\n - Add all the ingredients except the Creme de Mure and shake and strain over fresh ice into a double old fashioned glass and add the Creme de Mure.";
                    answer += "\n - Garnish with skewered blackberries or a a twist of lemon.";
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
                    answer += "\n\n - Traditionally served with no garnish but you can utilise a lemon twist or skewered raspberry if you like.";
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
                    var categories = new[] { "Caipiroska", "Chocolote Martini", "Lemon Drop Martini", "Espresso Martini", "Moscow Mule", "Go Back" };
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
                    answer += "\n\n - Garnish with a lime wheel or wedge";
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
                    answer += "\n\n - Garnish 3-4 coffe beans.";
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
                    var categories = new[] { "Cucumber & Jalapeno Margarita", "The Frozen Margarita", "Mexican Bulldog", "Go Back" };
                    var buttons = categories.Select(category => new[] { new KeyboardButton(category) })
                        .ToArray();
                    var selections = new ReplyKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(e.Message.Chat,
                        "*Select a cocktail to see its recipe.*",
                        ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId,
                        replyMarkup: selections);
                }
                else if (message.Equals("Cucumber & Jalapeno Margarita"))
                {
                    answer = "*--Cucumber & Jalapeno Margarita--*";
                    answer += "\nA spicy variation on a classic Margarita! The spicy jalapenos pair nicely with the cooling properties of fresh cucumber. Adds a little bit of a kick to your standard Margarita.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 45ml Tequila Bianco (3/2 oz) ";
                    answer += "\n - 15ml Triple Sec or Cointreau (1/2 oz)";
                    answer += "\n - 30mL Fresh lime juice (1 oz)";
                    answer += "\n - Cucumber";
                    answer += "\n - Jalapeno";
                    answer += "\n\n - Muddle cucumber and if you want it to be really hot you can also muddle jalapeno. Shake with ice and pour into a glass without straining.";
                    answer += "\n - Garnish with 2 slices of jalapeno and a slice of cucumber.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("The Frozen Margarita"))
                {
                    answer = "*--Cucumber & Jalapeno Margarita--*";
                    answer += "\nThe Frozen Margarita is the perfect summer, Tequila cocktail with versatility. Utilise a multitude of different fruits to customise your Margarita!";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 45ml Tequila (3/2 oz) ";
                    answer += "\n - 15ml Triple Sec or Cointreau (1/2 oz)";
                    answer += "\n - 30mL Fresh lime juice (1 oz)";
                    answer += "\n - Mango puree, pulp or cheeks";
                    answer += "\n\n - After you combine all the ingredients, blend for 20-30 seconds or until smooth.";
                    answer += "\n - Garnish with a lime wheel.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Mexican Bulldog"))
                {
                    answer = "*--Mexican Bulldog--*";
                    answer += "\nThe Mexican Bulldog a.k.a Frozen Coronita Margarita is the perfect beer cocktail for a hot summers day. It combines refreshing cold beer with a citrusy, tequila-based cocktail, the Margarita!";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Tequila (2 oz) ";
                    answer += "\n - 22.5ml Triple Sec or Cointreau (3/4 oz)";
                    answer += "\n - 37.5mL Fresh lime juice (5/4 oz)";
                    answer += "\n - Corona Beer";
                    answer += "\n\n - Combine all ingredients except the beer and blend with ice for 20-30 seconds or until smooth. Pour into a large glass and upturn your Corona beer into the glass";
                    answer += "\n - Garnish with a lime wedge.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Rum"))
                {

                    var categories = new[] { "Mojito", "Daiquiri", "Dark & Stormy", "Cuba Libre", "Pina Colada", "Go Back" };
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
                    answer += "\nThe origins of the Mojito are a little mysterious and some stories go as far back as the 16th century. We’re unsure of it’s true lineage but I know for sure, that it is the perfect refreshing summer cocktail and is a crowd favourite no matter the occasion.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml White Rum (2 oz)";
                    answer += "\n - 22.5ml Fresh lime juice (3/4 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    answer += "\n - 60ml Soda water (2 oz)";
                    answer += "\n - 6-8 Mint leaves";
                    answer += "\n\n - Add all ingredients except soda and gently muddle the mint. Pour into a high ball glass without straining and garnish with a spring of fresh mint";

                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Daiquiri"))
                {
                    answer = "*--Daiquiri--*";
                    answer += "\nThe Daiquiri would have to be one of the most iconic classics. It was first created in the 1890's and Bacardi was first call. Originally the drink was served in a tall glass packed with ice.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml White Rum (2 oz)";
                    answer += "\n - 30ml Fresh lime juice (1 oz)";
                    answer += "\n - 22.5ml Simple syrup (3/4 oz)";
                    answer += "\n\n - Garnish with a lime wheel.";

                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Dark & Stormy"))
                {
                    answer = "*--Dark & Stormy--*";
                    answer += "\nThe original trademarked version is held by the Gosling family. It calls for Gosling’s Bermudan rum but any dark rum to accompany a spicy ginger beer will work - just don’t substitute for ginger ale. Apparently many venues that utilise a different rum have had to resort to cleverly renaming the drink to Safe Harbour, in order to avoid litigation from the Gosling’s family.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml Dark Rum (2 oz)";
                    answer += "\n - 90ml Ginger Beer (3 oz)";
                    answer += "\n - Lime wedge";
                    answer += "\n\n - Garnish with a lime wedge.";

                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Cuba Libre"))
                {
                    answer = "*--Cuba Libre--*";
                    answer += "\nThe Cuba Libre is a simple, tasty cocktail steeped with history. The drink was created during the Spanish-American war and the words Cuba Libre translate to 'free Cuba'.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 60ml White Rum (2 oz)";
                    answer += "\n - Half a lime squeezed";
                    answer += "\n - 120ml Cola (4 oz)";
                    answer += "\n\n Squeeze half a lime into a high ball glass, add the rum, drop a lemon slice and finally top with cola.";

                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Pina Colada"))
                {
                    answer = "*--Pina Colada--*";
                    answer += "\nThe Pina Colada is the official cocktail of Puerto Rico and has been for over 35 years! It is also classified as an International Bartenders Association official cocktail and for good reason. Everyone loves a good pina colada especially in the summer months.";
                    answer += "\n\n* INGREDIENTS*";
                    answer += "\n - 45ml White Rum (2 oz)";
                    answer += "\n - 90ml Coconat cream (3 oz)";
                    answer += "\n - 180ml Pineapple juice (6 oz)";
                    answer += "\n\n - Combine all ingredients, add some crushed ice and blend for 20-30 seconds.";
                    answer += "\n - Garnish with a pineapple wedge.";
                    await botClient.SendTextMessageAsync(e.Message.Chat, answer, ParseMode.Markdown);
                }
                else if (message.Equals("Whiskey")) { }
                else if (message.Equals("Mix")) { }
                else if (message.Equals("Syrups")) { }
                else if (message.Equals("Alcohol free")) { }
                else if (message.Equals("Shots")) { }
                else if (message.Equals("Go back"))
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

            }
        }

        internal void Disconnect()
        {
            botClient.StopReceiving();
        }
    }
}