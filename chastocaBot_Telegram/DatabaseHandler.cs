using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace chastocaBot_Telegram
{
    class DatabaseHandler
    {
        private static string connectionString;
        public static void CreateTables()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbDirectory = Path.Combine(appDataPath, "Chastoca");
                string dbPath = Path.Combine(dbDirectory, "chastocaBotTelegram.db");
                connectionString = string.Format("Data Source={0}", dbPath);
                if (!File.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbDirectory);
                    SQLiteConnection.CreateFile(dbPath);

                    SQLiteConnection con = new SQLiteConnection(connectionString);
                    con.Open();

                    string sql;
                    SQLiteCommand cmd;

                    sql = @"CREATE TABLE Cocktails(cocktailName TEXT NOT NULL, recipe TEXT NOT NULL, type TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE CocktailTypes(cocktailType TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE Commands(question TEXT NOT NULL, reply TEXT NOT NULL, whoAdded TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE Locations(name TEXT NOT NULL, latitude REAL NOT NULL, longitude REAL NOT NULL, whoAdded TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE Reminders(time TEXT NOT NULL, text TEXT NOT NULL, whoAdded TEXT NOT NULL, name TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE Users(username TEXT NOT NULL, chatId TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();


                    con.Close();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #region Commands
        public static bool AddCommand(Command command)
        {
            if (!DoesUserHaveThisCommand(command.Question, command.WhoAdded))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Commands
               (question,reply,whoAdded) VALUES ('" + command.Question + "','" + command.Reply + "','" + command.WhoAdded + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteCommand(Command command)
        {
            if (DoesUserHaveThisCommand(command.Question, command.WhoAdded))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"DELETE from Commands WHERE question ='" + command.Question + "' AND whoAdded ='" + command.WhoAdded + "'";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }

        public static bool UpdateCommand(Command command)
        {
            int isSuccessful;
            if (DoesUserHaveThisCommand(command.Question, command.WhoAdded))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"UPDATE Commands SET reply='" + command.Reply + "', whoAdded = '" + command.WhoAdded + "' WHERE question ='" + command.Question + "'"; // hadi inş
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool DoesUserHaveThisCommand(string question, string whoAdded)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Commands WHERE question='" + question + "' AND whoADded='" + whoAdded + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static Command GetCommandFrom(string question, string whoAdded)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Commands WHERE question = '" + question + "' AND whoAdded = '" + whoAdded + "'";
                reader = cmd.ExecuteReader();
                Command command = new Command();
                while (reader.Read())
                {
                    command.Question = question;
                    command.Reply = reader[1].ToString().TrimEnd();
                    command.WhoAdded = reader[2].ToString().TrimEnd();
                }

                con.Close();
                return command;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new Command();
            }
        }
        public static int CountCommandsFrom(string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(*) FROM Commands WHERE whoADded='" + username + "';"; // nolur
                int count = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                return count;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return -1;
            }
        }
        public static List<Command> GetCommandsFrom(string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Commands WHERE whoAdded = '" + username + "'";
                reader = cmd.ExecuteReader();
                List<Command> commandList = new List<Command>();

                while (reader.Read())
                {
                    Command command = new Command();
                    command.Question = reader[0].ToString().TrimEnd();
                    command.Reply = reader[1].ToString().TrimEnd();
                    command.WhoAdded = username;
                    commandList.Add(command);
                }

                con.Close();
                return commandList;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<Command>();
            }
        }

        #endregion

        #region Cocktails
        public static bool AddCocktail(Cocktail cocktail)
        {
            if (!DoesExistInCocktails(cocktail.Name))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                int isSuccessful;
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Cocktails (cocktailName,recipe,type) VALUES ('" + cocktail.Name + "','" + cocktail.Recipe + "','" + cocktail.Type + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteCocktail(string cocktailName)
        {
            if (DoesExistInCocktails(cocktailName))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    int isSuccessful;
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"DELETE FROM Cocktails WHERE cocktailName='" + cocktailName + "'";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;

        }
        public static bool DoesExistInCocktails(string cocktailName)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Cocktails WHERE cocktailName='" + cocktailName + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static string GetCocktailRecipe(string cocktailName)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Cocktails WHERE cocktailName = '" + cocktailName + "'";
                reader = cmd.ExecuteReader();

                string answer = "";
                while (reader.Read())
                {
                    answer = reader[1].ToString();
                }

                con.Close();
                return answer;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return "";
            }

        }
        public static List<string> GetCocktailNames(string type)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Cocktails WHERE type = '" + type + "'";
                reader = cmd.ExecuteReader();
                var answer = new List<string>();

                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }

                con.Close();
                return answer;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }

        #endregion

        #region Cocktail types        
        public static bool DoesExistInCocktailTypes(string cocktailType)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM CocktailTypes WHERE cocktailType='" + cocktailType + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static bool AddCocktailType(string cocktailType)
        {
            if (!DoesExistInCocktailTypes(cocktailType))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"INSERT INTO CocktailTypes (cocktailType) VALUES ('" + cocktailType + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        internal static List<string> GetCocktailTypes()
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM CocktailTypes";
                reader = cmd.ExecuteReader();
                var answer = new List<string>();

                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }

                con.Close();
                return answer;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }
        #endregion        

        #region Locations        
        public static bool AddLocation(Location location)
        {
            if (!DoesExistInLocations(location.Name, location.WhoAdded))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    string lat = location.Latitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
                    string lon = location.Longitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
                    cmd.CommandText = @"INSERT INTO Locations (name,latitude,longitude,whoAdded) VALUES ('" + location.Name + "','" + lat + "','" + lon + "','" + location.WhoAdded + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteLocation(string locationName, string username)
        {
            if (DoesExistInLocations(locationName, username))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"DELETE FROM Locations WHERE name='" + locationName + "' AND whoAdded='" + username + "'";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static Location GetLocation(string locationName, string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Locations WHERE name = '" + locationName + "' AND whoAdded='" + username + "'";
                reader = cmd.ExecuteReader();

                Location location = new Location();

                while (reader.Read())
                {
                    CultureInfo numberFormat = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    numberFormat.NumberFormat.CurrencyDecimalSeparator = ".";
                    location.Name = locationName;
                    location.Latitude = float.Parse(reader[1].ToString().TrimEnd(), NumberStyles.Any, numberFormat); //reader[1].ToString();
                    location.Longitude = float.Parse(reader[2].ToString().TrimEnd(), NumberStyles.Any, numberFormat);
                    location.WhoAdded = reader[3].ToString().TrimEnd();
                }

                con.Close();
                return location;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new Location();
            }

        }
        public static bool DoesExistInLocations(string locationName, string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Locations WHERE name='" + locationName + "' AND whoAdded='" + username + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static List<string> GetLocationNamesFrom(string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Locations WHERE whoAdded = '" + username + "'";
                reader = cmd.ExecuteReader();
                var answer = new List<string>();

                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }

                con.Close();
                return answer;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }
        #endregion

        #region Users
        public static bool DoesExistInUsers(string username, string chatId)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Users WHERE username='" + username + "' AND chatId='" + chatId + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static bool AddUser(User newUser)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                if (!DoesExistInUsers(newUser.Username, newUser.ChatId))
                {
                    int isSuccessful;
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();

                    cmd.CommandText = @"INSERT INTO Users (username,chatId) VALUES ('" + newUser.Username + "','" + newUser.ChatId + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static User GetUser(string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users WHERE username = '" + username + "'";
                reader = cmd.ExecuteReader();

                User user = new User();

                while (reader.Read())
                {
                    user.Username = username;
                    user.ChatId = reader[1].ToString();
                }

                con.Close();
                return user;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new User();
            }
        }
        public static List<User> GetUsers()
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {

                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users";
                reader = cmd.ExecuteReader();

                List<User> Users = new List<User>();

                while (reader.Read())
                {
                    User user = new User
                    {
                        Username = reader[0].ToString().TrimEnd(),
                        ChatId = reader[1].ToString().TrimEnd()
                    };
                    Users.Add(user);
                }

                con.Close();
                return Users;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<User>();
            }
        }

        #endregion

        #region Reminders
        public static bool AddReminder(Reminder reminder)
        {
            if (!DoesExistInReminders(reminder))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    int isSuccessful;
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();

                    cmd.CommandText = @"INSERT INTO Reminders (time, text, whoAdded, name) VALUES ('" + reminder.Date + "','" + reminder.Text + "','" + reminder.WhoAdded + "','" + reminder.Name + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteReminder(Reminder reminder)
        {
            if (DoesExistInReminders(reminder))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    int isSuccessful;
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"DELETE FROM Reminders WHERE name='" + reminder.Name + "' AND whoAdded='" + reminder.WhoAdded + "'";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static List<Reminder> GetRemindersFrom(string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Reminders WHERE whoAdded = '" + username + "'";
                reader = cmd.ExecuteReader();
                List<Reminder> reminderList = new List<Reminder>();

                while (reader.Read())
                {
                    Reminder reminder = new Reminder
                    {
                        Date = DateTime.Parse(reader[0].ToString().TrimEnd()),
                        Text = reader[1].ToString().TrimEnd(),
                        WhoAdded = reader[2].ToString().TrimEnd(),
                        Name = reader[3].ToString().TrimEnd()
                    };
                    reminderList.Add(reminder);
                }

                con.Close();
                return reminderList;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new List<Reminder>();
            }

        }
        public static bool DoesExistInReminders(Reminder reminder)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Reminders WHERE time='" + reminder.Date + "' AND text='" + reminder.Text + "' AND whoAdded='" + reminder.WhoAdded + "' AND name='" + reminder.Name + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static bool DoesExistInReminders(string reminderName, string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Reminders WHERE  whoAdded='" + username + "' AND name='" + reminderName + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static Reminder GetSoonestReminder()
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Reminders";
                reader = cmd.ExecuteReader();

                Reminder soonestReminder = new Reminder
                {
                    Date = new DateTime(2025, 9, 19)
                };
                while (reader.Read())
                {
                    Reminder reminder = new Reminder
                    {
                        Date = DateTime.Parse(reader[0].ToString().TrimEnd()),
                        Text = reader[1].ToString().TrimEnd(),
                        WhoAdded = reader[2].ToString().TrimEnd(),
                        Name = reader[3].ToString().Trim()
                    };

                    if (DateTime.Compare(reminder.Date, soonestReminder.Date) < 0)
                        soonestReminder = reminder;
                }

                con.Close();
                return soonestReminder;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new Reminder();
            }
        }
        public static Reminder GetReminder(string reminderName, string username)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Reminders WHERE name='" + reminderName + "' AND whoAdded='" + username + "'";
                reader = cmd.ExecuteReader();

                Reminder reminder = new Reminder();
                while (reader.Read())
                {
                    reminder.Date = DateTime.Parse(reader[0].ToString().TrimEnd());
                    reminder.Text = reader[1].ToString().TrimEnd();
                    reminder.WhoAdded = reader[2].ToString().TrimEnd();
                    reminder.Name = reader[3].ToString().TrimEnd();
                }

                con.Close();
                return reminder;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new Reminder();
            }
        }

        #endregion
    }
}