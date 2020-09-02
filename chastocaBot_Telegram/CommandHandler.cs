using System;
using System.Data.SqlClient;
using Telegram.Bot.Types;

namespace chastocaBot_Telegram
{
    class CommandHandler
    {
        private static SqlCommand sqlCommand;
        private static SqlConnection connection;
        private static SqlDataReader reader;
        private static readonly string connecString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=chastocaBotTelegram;Integrated Security=True"; //   localhost\\SQLEXPRESS  --- DESKTOP-MT1SCOE\\ARDA

        internal static string AddCounter(string command, string userID)
        {
            // is counter command exist
            if (!DoesExistInCounters(command, userID))
            {
                // if doesnt exists, create one 
                int isSuccessful;
                Console.WriteLine("Adding the counter into database.");
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
                        CommandText = "INSERT INTO Counters (command,counter,userId) VALUES ('" + command + "','" + 1 + "','" + userID + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();

                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't add the counter into database.");
                        return "0";
                    }
                    else
                        return FindCounterAnswer(command, userID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.HelpLink);
                    return "0";
                }
            }
            else // if counter exists
            {
                int counter = Convert.ToInt32(FindCounterAnswer(command, userID)) + 1;

                if (UpdateCounter(command, counter.ToString(), userID))
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
        public static bool UpdateCounter(string command, string counter, string userID)
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

                    CommandText = "UPDATE Counters SET command='" + command + "',counter='" + counter + "' WHERE command ='" + command + "' AND userId='" + userID + "'"
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
        public static bool DoesExistInCounters(string command, string userID)
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Counters WHERE (command = @command) AND (userId = @userId)", connection);
            checkCommand.Parameters.AddWithValue("@command", command);
            checkCommand.Parameters.AddWithValue("@userId", userID);

            connection.Open();
            int commandExist = (int)checkCommand.ExecuteScalar();
            connection.Close();
            if (commandExist > 0)
                return true;            
            else
            {
                Console.WriteLine("Counter is not in the database.");
                return false;
            }

        }
        public static string FindCounterAnswer(string command, string userID)
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            connection.Open();
            sqlCommand = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM Counters WHERE command='" + command + "' AND userId='" + userID + "'"
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



        #region Commands
        public static bool AddCommand(string command, string answer)
        {
            if (!DoesExistInCommands(command))
            {
                int isSuccessful;
                Console.WriteLine("Adding the command into database.");
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Commands (command,answer) VALUES ('" + command + "','" + answer + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't add the command into database.");
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
            else
                return false;
        }
        public static bool ChangeCommand(string command, string updatedCommand, string answer)
        {
            int isSuccessful;
            if (DoesExistInCommands(command))
            {
                try
                {
                    Console.WriteLine("Changing the command " + command + " to " + updatedCommand + " and the answer is " + answer);
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,

                        CommandText = "UPDATE Commands  SET command='" + updatedCommand + "',answer='" + answer + "' WHERE command ='" + command + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't change the command in database.");
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
            else
            {
                return false;
            }
        }
        public static bool DeleteCommand(string komut)
        {
            if (DoesExistInCommands(komut))
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
                        CommandText = "DELETE FROM Commands WHERE command='" + komut + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't delete the command from database.");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Command deleted from database successfully.");
                        return true;
                    }
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
            {
                Console.WriteLine("Command is in the database.");
                return true;
            }
            else
            {
                Console.WriteLine("Command is not in the database.");
                return false;
            }

        }
        public static string FindAnswer(string command)
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
    }
}
