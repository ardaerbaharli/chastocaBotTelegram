# chastocaBotTelegram
It is a Telegram bot that I built for mostly me and my friends.

## !Features

## There are 4 different ranks: 
 - **Folk:**- Starting rank.
 - **Berserker:** Premium user rank, has access to !commands and !berserkercommands.
 - **Gatekeeper:** Moderator, has access to !admincommands.
 - **King:** @ardaerbaharli
 
- !commands
- !berserkercommands
- !admincommands
- !cocktails >> Shows all the registered cocktail types and when you pick one of them, it shows all the cocktails with the specified type.
  
  #### Fall Guys Commands (To user !win and !wincount you should have a username on Telegram.)
- !win >> Adds 1 to the win counter.
- !leaderboard >> Shows top 5 player with the highest score.
- !wincount >> Shows how many wins you have.

### Berserker commands
## There are some commands that only berserkers can use. By getting the Berserker rank:

**!addcommand**>>> You can add 5 personal custom commands that only you will see the reply of that command.
  >> Use: !addcommand commandName reply
    >> From now on when you write commandName, bot will reply with you defined reply. (Command name have to be 1 word and reply has 800 character limit.)

**!deletecommand **>>> 
 >> Use: !deletecommand commandName  
   
**!updatecommand** >>>
 >> Use: !updatecommand commandName newReply

**!mycommands** >>> Lists all of your custom commands you added.

**!addlocation** >>> You can add 5 personal locations that only you will see.
 >> Use: !addlocation locationName latitude longitude
 >> You can find latitude and longitude of your location:
 > Google Maps and Yandex Maps by:
  - Right click or long press on the map and in the shown menu click "What is Here?"
 > Apple Maps
   - Long press on the map and scroll down.

**!deletelocation**
>> Use: !deletelocation locationName

**!mylocations** >>> Lists all of your custom locations you added.

  #### Admin only comamnds
- !addlocation
- !deletelocation
- !addcommand
- !deletecommand
- !updatecommand
- !deletecommandsfrom <username>
- !giverank <username>
- !ranks
- !listusers 
- !announce <rankToAnnounce> <announce> 
(Folk = All,Berserker = Except folk, Gatekeeper = Except berserker and folk)
- !mylocations
- !mycommands

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
        - whoCanAccess >> nvarchar(100)
        - answer >> nvarchar(200)
        - whoAdded >> nvarchar(100)
    - Counters
        - command >> nvarchar(100)
        - counter >> int
        - userId >> nvarchar(70)
    - Locations 
        - name >> nvarchar(100)
        - latitude >> float
        - longitude >> float
        - whoAdded >> nvarchar(100)
    - Users
        - username >> nvarchar(100)
        - chatId >> nvarchar(20)
        - rank >> nvarchar(50)
