# chastocaBotTelegram
It is a Telegram bot that I built for mostly me and my friends.

## !Features

- !help \>> Shows all the commands in the Commands table.
- !cocktails >> Shows all the registered cocktail types and when you pick one of them, it shows all the cocktails with the specified type.
  
  #### Fall Guys Commands (To user !win and !wincount you should have a username on Telegram.)
- !win >> Adds 1 to the win counter.
- !leaderboard >> Shows top 5 player with the highest score.
- !wincount >> Shows how many wins you have.

  #### Admin only comamnds
  
- loc >> Shows registered locations with reply markup and when you click one of the buttons, it will send you the location of that venue. 
- !announce <announce> >> Sends *announce* message to every user.
- !addlocation <name> <latitude> <longitude> >> Adds the specified coordinates to database with the given name.
- !deletelocation <name> >> Removes the record given name from database.
- !addcommand <command> <reply> >> Adds the command to the database. (!addcommand !github github.com/ardaerbaharli , and whenever someone writes !github to the bot, it will reply with github link.)
- !deletecommand <command> >> Deletes the command from database.
  
- !addcocktail <cocktailName> <type> <recipe> >> Adds the cocktail to the database.
  - cocktailName: Name of the cocktail.
  - Type: Vodka, Gin, Alcohol-free, syrup etc.  
  - Recipe: Detailed recipe of how to make the cocktail.
- !deletecocktail <cocktailName> >> Deletes the cocktail from database.
  
  
 ### To make it work you need to do a few things: 
 - You need to change the path in LogHandler.cs line 12 and sql connection string in CommandHandler.cs line 12 to use the bot.
 - Create 6 different tables:
    - Cocktails 
        - cocktailName >> nvarchar(100)
        - recipe >> nvarchar(800)
        - type >> nvarchar(100)
    - CocktailTypes
        - cocktailType >> nvarchar(100)
    - Commands
        - command >> nvarchar(200)
        - answer >> nvarchar(200)
    - Counters
        - command >> nvarchar(100)
        - counter >> int
        - userId >> nvarchar(70)
    - Locations 
        - name >> nvarchar(100)
        - latitude >> float
        - longitude >> float
    - Users
        - username >> nvarchar(100)
        - chatId >> nvarchar(20)
