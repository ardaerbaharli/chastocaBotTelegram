using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace chastocaBot_Telegram
{
    class DatabaseHandler
    {
        private static SqlCommand sqlCommand;
        private static SqlConnection connection;
        private static SqlDataReader reader;
        private static readonly string connecString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=chastocaBotTelegram;Integrated Security=True"; // localhost\\SQLEXPRESS  --- DESKTOP-MT1SCOE\\ARDA

        #region Counters
        internal static string AddCounter(string command, string username)
        {
            // is counter command exist
            if (!DoesExistInCounters(command, username))
            {
                // if doesnt exists, create one 
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        // add to command and win count(starting as 1) to database
                        CommandText = "INSERT INTO Counters (command,counter,userId) VALUES ('" + command + "','" + 1 + "','" + username + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();

                    if (isSuccessful == 0)
                        return "0";
                    else
                        return FindCounterAnswer(command, username);
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return "0";
                }

            }
            else // if counter exists
            {
                int counter = Convert.ToInt32(FindCounterAnswer(command, username)) + 1;
                //  Counter counter = new Counter();
                // counter.Command = 
                if (UpdateCounter(command, counter.ToString(), username))
                    return counter.ToString();
                else
                    return "0";
            }
        }
        internal static string[,] GetLeaderboard(string command)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT TOP 5 * FROM Counters WHERE command='" + command + "' ORDER BY counter DESC"
                };
                reader = sqlCommand.ExecuteReader();
                string[,] leaderboard = new string[5, 3];
                int row = 0, col = 0;
                while (reader.Read())
                {
                    string username = reader[2].ToString().TrimEnd();
                    string score = reader[1].ToString().TrimEnd();
                    if (BotControl.CanAccess(GetUser(username), "Berserker"))
                    {
                        leaderboard[row, col] = username;
                        leaderboard[row, col + 1] = score;
                        row++;
                    }
                }
                connection.Close();

                return leaderboard;
            }
            catch (Exception ex)
            {
                LogHandler.ReportCrash(ex);
                connection.Close();
                string[,] crash = new string[1, 1];
                return crash;
            }

        }
        public static bool UpdateCounter(string command, string counter, string username)
        {
            int isSuccessful;
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                sqlCommand = new SqlCommand
                {
                    Connection = connection,

                    CommandText = "UPDATE Counters SET command='" + command + "',counter='" + counter + "' WHERE command ='" + command + "' AND userId='" + username + "'"
                };
                connection.Open();
                isSuccessful = sqlCommand.ExecuteNonQuery();
                connection.Close();
                if (isSuccessful == 0)
                {
                    Console.WriteLine("Couldn't change the counter in database.");
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static bool DoesExistInCounters(string command, string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Counters WHERE (command = @command) AND (userId = @userId)", connection);
                checkCommand.Parameters.AddWithValue("@command", command);
                checkCommand.Parameters.AddWithValue("@userId", username);

                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogHandler.ReportCrash(ex);
                connection.Close();
                return false;
            }
        }
        public static string FindCounterAnswer(string command, string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Counters WHERE command='" + command + "' AND userId='" + username + "'"
                };
                reader = sqlCommand.ExecuteReader();
                string answer = "";
                while (reader.Read())
                {
                    answer = reader[1].ToString();
                }
                connection.Close();
                if (answer.Length < 1)
                    answer = "0";
                return answer;
            }
            catch (Exception ex)
            {
                LogHandler.ReportCrash(ex);
                connection.Close();
                string crash = "-1";
                return crash;
            }
        }

        #endregion

        #region Commands
        public static bool AddCommand(Command command)
        {
            if (!DoesUserHaveThisCommand(command))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Commands (question,whoCanAccess,reply,whoAdded) VALUES ('" + command.Question + "','" + command.WhoCanAccess + "','" + command.Reply + "','" + command.WhoAdded + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteCommand(Command command)
        {
            if (DoesUserHaveThisCommand(command))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "DELETE FROM Commands WHERE question ='" + command.Question + "' AND whoAdded ='" + command.WhoAdded + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
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
            if (DoesUserHaveThisCommand(command))
            {
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "UPDATE Commands SET reply='" + command.Reply + "', whoCanAccess = '" + command.WhoCanAccess + "' WHERE question ='" + command.Question + "'" // hadi inş
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool DoesExistInCommands(string question)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (question = @question)", connection);
                checkCommand.Parameters.AddWithValue("@question", question);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static bool DoesUserHaveThisCommand(Command command)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (question = @question) AND (whoAdded = @whoAdded)", connection);
                checkCommand.Parameters.AddWithValue("@question", command.Question);
                checkCommand.Parameters.AddWithValue("@whoAdded", command.WhoAdded);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static Command GetCommand(string question)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Commands WHERE question='" + question + "'"// AND whoAdded='" + whoAdded + "'"
                };
                reader = sqlCommand.ExecuteReader();
                Command command = new Command();
                while (reader.Read())
                {
                    command.Question = question;
                    command.WhoCanAccess = reader[1].ToString().TrimEnd();
                    command.Reply = reader[2].ToString().TrimEnd();
                }
                connection.Close();
                return command;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new Command();
            }
        }
        public static Command GetCommandFrom(string question, string whoAdded)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Commands WHERE question='" + question + "' AND whoAdded='" + whoAdded + "'"
                };
                reader = sqlCommand.ExecuteReader();
                Command command = new Command();
                while (reader.Read())
                {
                    command.Question = question;
                    command.WhoCanAccess = reader[1].ToString().TrimEnd();
                    command.Reply = reader[2].ToString().TrimEnd();

                }
                connection.Close();
                return command;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new Command();
            }
        }
        public static bool DeletesCommandsFrom(string username)
        {
            int isSuccessful;
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "DELETE FROM Commands WHERE whoAdded ='" + username + "'"
                };
                connection.Open();
                isSuccessful = sqlCommand.ExecuteNonQuery();
                connection.Close();
                if (isSuccessful == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static int CountCommandsFrom(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (whoAdded = @whoAdded)", connection);
                checkCommand.Parameters.AddWithValue("@whoAdded", username);
                connection.Open();
                int count = (int)checkCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return -1;
            }

        }
        public static List<Command> GetCommandsFrom(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Commands WHERE whoAdded='" + username + "'"
                };
                reader = sqlCommand.ExecuteReader();

                List<Command> commandList = new List<Command>();

                while (reader.Read())
                {
                    Command command = new Command();
                    command.Question = reader[0].ToString().TrimEnd();
                    command.WhoCanAccess = reader[1].ToString().TrimEnd();
                    command.Reply = reader[2].ToString().TrimEnd();
                    command.WhoAdded = username;
                    commandList.Add(command);
                }
                connection.Close();
                return commandList;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new List<Command>();
            }
        }
        public static int CountCommandsWhoCanAccess(string rank)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (whoCanAccess = @whoCanAccess)", connection);
                checkCommand.Parameters.AddWithValue("@whoCanAccess", rank);
                connection.Open();
                int count = (int)checkCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return -1;
            }

        }
        public static List<Command> GetCommandsWhoCanAccess(string rank)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Commands WHERE whoCanAccess ='" + rank + "'"
                };
                reader = sqlCommand.ExecuteReader();

                List<Command> commandList = new List<Command>();

                while (reader.Read())
                {
                    Command command = new Command
                    {
                        Question = reader[0].ToString().TrimEnd(),
                        WhoCanAccess = reader[1].ToString().TrimEnd(),
                        Reply = reader[2].ToString().TrimEnd(),
                        WhoAdded = reader[3].ToString().TrimEnd()
                    };
                    commandList.Add(command);
                }
                connection.Close();
                return commandList;
            }
            catch (Exception ex)
            {
                connection.Close();
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
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Cocktails (cocktailName,recipe,type) VALUES ('" + cocktail.Name + "','" + cocktail.Recipe + "','" + cocktail.Type + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
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
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "DELETE FROM Cocktails WHERE cocktailName='" + cocktailName + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;

        }
        public static bool DoesExistInCocktails(string cocktailName)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Cocktails WHERE (cocktailName = @cocktailName)", connection);
                checkCommand.Parameters.AddWithValue("@cocktailName", cocktailName);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static string GetCocktailRecipe(string cocktailName)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Cocktails WHERE cocktailName='" + cocktailName + "'"
                };
                reader = sqlCommand.ExecuteReader();
                string answer = "";
                while (reader.Read())
                {
                    answer = reader[1].ToString();
                }
                connection.Close();
                return answer;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return "";
            }

        }
        public static List<string> GetCocktailNames(string type)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Cocktails WHERE type='" + type + "'"
                };
                reader = sqlCommand.ExecuteReader();
                var answer = new List<string>();
                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }
                connection.Close();

                return answer;

            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }

        #endregion

        #region Cocktail types        
        public static bool DoesExistInCocktailTypes(string cocktailType)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM CocktailTypes WHERE (cocktailType = @cocktailType)", connection);
                checkCommand.Parameters.AddWithValue("@cocktailType", cocktailType);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
        public static bool AddCocktailType(string cocktailType)
        {
            if (!DoesExistInCocktailTypes(cocktailType))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO CocktailTypes (cocktailType) VALUES ('" + cocktailType + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        internal static List<string> GetCocktailTypes()
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM CocktailTypes"
                };
                reader = sqlCommand.ExecuteReader();
                var answer = new List<string>();
                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }
                connection.Close();

                return answer;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }
        #endregion        

        #region Locations        
        public static bool AddLocation(Location location)
        {
            if (!DoesExistInLocations(location.Name))
            {
                string lat = location.Latitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
                string lon = location.Longitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Locations (name,latitude,longitude,whoAdded) VALUES ('" + location.Name + "','" + lat + "','" + lon + "','" + location.WhoAdded + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteLocation(string name)
        {
            if (DoesExistInLocations(name))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "DELETE FROM Locations WHERE name='" + name + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static Location GetLocation(string name)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Locations WHERE name='" + name + "'"
                };
                reader = sqlCommand.ExecuteReader();


                Location location = new Location();
                while (reader.Read())
                {
                    CultureInfo numberFormat = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    numberFormat.NumberFormat.CurrencyDecimalSeparator = ".";
                    location.Name = name;
                    location.Latitude = float.Parse(reader[1].ToString().TrimEnd(), NumberStyles.Any, numberFormat); //reader[1].ToString();
                    location.Longitude = float.Parse(reader[2].ToString().TrimEnd(), NumberStyles.Any, numberFormat);
                    location.WhoAdded = reader[3].ToString().TrimEnd();

                }
                connection.Close();
                return location;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new Location();
            }

        }
        public static bool DoesExistInLocations(string name)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Locations WHERE (name)='" + name + "'", connection);
                // checkCommand.Parameters.AddWithValue("@name", name);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static List<string> GetLocationNamesFrom(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Locations WHERE whoAdded='" + username + "'"
                };
                reader = sqlCommand.ExecuteReader();
                var answer = new List<string>();
                while (reader.Read())
                {
                    answer.Add(reader[0].ToString().TrimEnd());
                }
                connection.Close();

                return answer;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new List<string>();
            }

        }
        public static int CountLocationsFrom(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Locations WHERE (whoAdded = @whoAdded)", connection);
                checkCommand.Parameters.AddWithValue("@whoAdded", username);
                connection.Open();
                int count = (int)checkCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return 0;
            }

        }
        #endregion

        #region Users
        public static bool DoesExistInUsers(string username, string chatId)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Users WHERE (username = @username) AND (chatId = @chatId)", connection);
                checkCommand.Parameters.AddWithValue("@username", username);
                checkCommand.Parameters.AddWithValue("@chatId", chatId);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static bool AddUser(User newUser)
        {
            if (!DoesExistInUsers(newUser.Username, newUser.ChatId))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Users (username,chatId,rank) VALUES ('" + newUser.Username + "','" + newUser.ChatId + "','" + newUser.Rank + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool UpdateUser(User user)
        {
            int isSuccessful;
            if (DoesExistInUsers(user.Username, user.ChatId))
            {
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "UPDATE Users SET rank='" + user.Rank + "' WHERE username ='" + user.Username + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static User GetUser(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Users WHERE username='" + username + "'"
                };
                reader = sqlCommand.ExecuteReader();

                User user = new User();
                while (reader.Read())
                {
                    user.Username = username;
                    user.ChatId = reader[1].ToString();
                    user.Rank = reader[2].ToString();
                }
                connection.Close();
                return user;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new User();
            }

        }
        internal static List<User> GetUsers()
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Users"
                };
                reader = sqlCommand.ExecuteReader();

                List<User> Users = new List<User>();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Username = reader[0].ToString().TrimEnd(),
                        ChatId = reader[1].ToString().TrimEnd(),
                        Rank = reader[2].ToString().TrimEnd()
                    };
                    Users.Add(user);
                }
                connection.Close();

                return Users;
            }
            catch (Exception ex)
            {
                connection.Close();
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
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };

                    String query = "INSERT INTO Reminders (time, text, whoAdded) VALUES (@time, @text, @whoAdded)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@time", reminder.Date);
                        command.Parameters.AddWithValue("@text", reminder.Text);
                        command.Parameters.AddWithValue("@whoAdded", reminder.WhoAdded);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        connection.Close();
                        // Check Error
                        if (result < 0)
                            return false;
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    connection.Close();
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
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "DELETE FROM Reminders WHERE text='" + reminder.Text + "'AND whoAdded='" + reminder.WhoAdded + "'"
                    };
                    connection.Open();
                    int isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static List<Reminder> GetRemindersFrom(string whoAdded)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Reminders WHERE whoAdded='" + whoAdded + "'"
                };
                reader = sqlCommand.ExecuteReader();

                List<Reminder> reminderList = new List<Reminder>();


                while (reader.Read())
                {
                    Reminder reminder = new Reminder
                    {
                        Text = reader[0].ToString().TrimEnd(),
                        Date = DateTime.Parse(reader[1].ToString().TrimEnd()),
                        WhoAdded = reader[2].ToString().TrimEnd()
                    };
                    reminderList.Add(reminder);
                }
                connection.Close();
                return reminderList;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new List<Reminder>();
            }

        }
        public static bool DoesExistInReminders(Reminder reminder)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Reminders WHERE (time = @time) AND (text = @text) AND (whoAdded = @whoAdded) ", connection);
                checkCommand.Parameters.AddWithValue("@time", reminder.Date);
                checkCommand.Parameters.AddWithValue("@text", reminder.Text);
                checkCommand.Parameters.AddWithValue("@whoAdded", reminder.WhoAdded);
                connection.Open();
                int commandExist = (int)checkCommand.ExecuteScalar();
                connection.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }

        }
        public static Reminder GetSoonestReminder()
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                connection.Open();
                sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Reminders"
                };
                reader = sqlCommand.ExecuteReader();

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
                        WhoAdded = reader[2].ToString().TrimEnd()
                    };

                    if (DateTime.Compare(reminder.Date, soonestReminder.Date) < 0)
                        soonestReminder = reminder;
                }
                connection.Close();

                return soonestReminder;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return new Reminder();
            }
        }
        public static int CountRemindersFrom(string username)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = connecString
                };
                using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Reminders WHERE (whoAdded = @whoAdded)", connection);
                checkCommand.Parameters.AddWithValue("@whoAdded", username);
                connection.Open();
                int count = (int)checkCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                connection.Close();
                LogHandler.ReportCrash(ex);
                return 0;
            }
        }

        #endregion       
    }
}