using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace chastocaBot_Telegram
{
    class DatabaseHandler
    {
        private static SqlCommand sqlCommand;
        private static SqlConnection connection;
        private static SqlDataReader reader;
        private static readonly string connecString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=chastocaBotTelegram;Integrated Security=True"; //   localhost\\SQLEXPRESS  --- DESKTOP-MT1SCOE\\ARDA

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
                    Console.WriteLine(ex.HelpLink);
                    return "0";
                }
            }
            else // if counter exists
            {
                int counter = Convert.ToInt32(FindCounterAnswer(command, username)) + 1;

                if (UpdateCounter(command, counter.ToString(), username))
                    return counter.ToString();
                else
                    return "0";
            }

        }
        internal static string[,] GetLeaderboard(string command)
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
            string[,] answer = new string[5, 3];
            int i = 0, j = 0;
            while (reader.Read())
            {
                answer[i, j] = reader[2].ToString().TrimEnd();
                answer[i, j + 1] = reader[1].ToString().TrimEnd();
                i++;
            }
            connection.Close();

            return answer;
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
                Console.WriteLine(ex.HelpLink);
                return false;
            }

        }
        public static bool DoesExistInCounters(string command, string username)
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
        public static string FindCounterAnswer(string command, string username)
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

        #endregion

        #region Commands
        public static bool AddCommand(string command, string reply)
        {
            if (!DoesExistInCommands(command))
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
                        CommandText = "INSERT INTO Commands (command,answer) VALUES ('" + command + "','" + reply + "')"
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;
        }       
        public static bool DeleteCommand(string command)
        {
            if (DoesExistInCommands(command))
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
                        CommandText = "DELETE FROM Commands WHERE command='" + command + "'"
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;

        }
        public static bool ChangeCommand(string command, string reply)
        {
            int isSuccessful;
            if (DoesExistInCommands(command))
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

                        CommandText = "UPDATE Commands SET answer='" + reply + "' WHERE command ='" + command + "'"
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool DoesExistInCommands(string command)
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (command = @command)", connection);
            checkCommand.Parameters.AddWithValue("@command", command);
            connection.Open();
            int commandExist = (int)checkCommand.ExecuteScalar();
            connection.Close();
            if (commandExist > 0)
                return true;
            else
                return false;

        }       
        public static string GetReply(string command)
        {

            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            connection.Open();
            sqlCommand = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM Commands WHERE command='" + command + "'"
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

        #endregion

        #region Cocktails
        public static bool AddCocktail(string cocktailName, string recipe, string alcoholcategory)
        {
            if (!DoesExistInCocktails(cocktailName))
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
                        CommandText = "INSERT INTO Cocktails (cocktailName,recipe,type) VALUES ('" + cocktailName + "','" + recipe + "','" + alcoholcategory + "')"
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
                    Console.WriteLine(ex.HelpLink);
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;

        }
        public static bool DoesExistInCocktails(string cocktailName)
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
        public static string GetCocktailRecipe(string cocktailName)
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
        internal static List<string> GetCocktailNames(string type)
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

        #endregion

        #region Cocktail types        
        public static bool DoesExistInCocktailTypes(string cocktailType)
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;
        }
        internal static List<string> GetCocktailTypes()
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
        #endregion        

        #region Locations        
        public static bool AddLocation(string name, double latitude, double longitude)
        {
            if (!DoesExistInLocations(name))
            {
                string lat = latitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
                string lon = longitude.ToString("#.######", System.Globalization.CultureInfo.InvariantCulture);
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
                        CommandText = "INSERT INTO Locations (name,latitude,longitude) VALUES ('" + name + "','" + lat+ "','" + lon + "')"
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
                    Console.WriteLine(ex.StackTrace);
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;
        }
        public static Location GetLocation(string name)
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
                //Location l = new Location();
                location.Name = name;
                location.Latitude = reader[1].ToString();
                location.Longitude = reader[2].ToString();
                
            }
            connection.Close();
            return location;
        }
        public static bool DoesExistInLocations(string name)
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
        public static List<string> GetLocationNames()
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            connection.Open();
            sqlCommand = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM Locations"
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

        #endregion

        #region Users

        public static bool DoesExistInUsers(string username,string chatId)
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
         public static bool AddUser(string username, string chatId)
        {
            if (!DoesExistInUsers(username,chatId))
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
                        CommandText = "INSERT INTO Users (username,chatId) VALUES ('" + username + "','" + chatId + "')"
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
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;
        }
        internal static List<User> GetUsers()
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
                    ChatId = reader[1].ToString().TrimEnd()
                };
                Users.Add(user);
            }
            connection.Close();

            return Users;
        }
        #endregion
    }
}
