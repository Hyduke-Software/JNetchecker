﻿
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;


namespace JNetchecker
{
    public class DataAccess
    {
        public static string RetrieveDatabaseFilePath()
        {
            //reads the JSON file with config values
            return Config.readConfig().DatabaseFilepath;
        }

        public static void dropTable()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS hosts";
                delTableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static SqliteConnection connection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            // connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
           connectionStringBuilder.DataSource = RetrieveDatabaseFilePath();
            SqliteConnection connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            return connection;

        }

        public static void storeTicket(string hostname, string ticketText, string poster, int active)
        {
            //stores ticket, and sets the active flag in hosts
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                 connectionC.Open();
                var delTableCmd = connectionC.CreateCommand();
                delTableCmd.CommandText = $"INSERT INTO tickets (Hostname, Text, Poster, Active) VALUES('{hostname}','{ticketText}','{poster}',{active})";
                delTableCmd.ExecuteNonQuery();
                connectionC.Close();
            }
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                var delTableCmd = connectionC.CreateCommand();
                delTableCmd.CommandText = $"UPDATE hosts set activeticket = '{active}' WHERE name = '{hostname}'";
                delTableCmd.ExecuteNonQuery();
                connectionC.Close();
            }

        }

        public static bool checkHostNameUniqueness(string hostname)
        {
            //returns a bool on if hostname is already in use
            bool result = true;
            using (var connectionC = connection()) //uses one subroutine for the connection string  builder
            {
                connectionC.Open();
                var selectCmd = connectionC.CreateCommand();
                selectCmd.CommandText = "SELECT COUNT(1) FROM hosts WHERE name = '"+ hostname + "'";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var output = reader.GetString(0);
                        //attempts to parse result to bool
                        try
                        {
                            result = Convert.ToBoolean(output);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                connectionC.Close();
            }
            return result;
        }

            public static List<TicketList> readTickets(string hostname)
        {
            List<TicketList> tickets = new List<TicketList>();
                using (var connectionC = connection())
                {
                    connectionC.Open();
                    var selectCmd = connectionC.CreateCommand();
                    selectCmd.CommandText = $"SELECT * FROM tickets where Hostname = '{hostname}' ORDER BY ID desc";
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new TicketList() { Hostname = (string)reader["Hostname"], ID = ((Int64)reader["ID"]), Poster = (string)reader["Poster"], Text = (string)reader["Text"], Timestamp = (string)reader["Timestamp"], Active = ((Int64)reader["Active"]) });
                        }
                    }
                    connectionC.Close();
                }
                return tickets;

        }

        public static void addHostToDatabase(List<host> newHost)
        {
            //06/12/20 even though it takes a list it only imports the first. To be removed.

            //   connectionStringBuilder.DataSource = "./SqliteDB.db";
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();

                using (var transaction = connectionC.BeginTransaction())
                {
                    var insertCmd = connectionC.CreateCommand();

                    insertCmd.CommandText = $"INSERT INTO hosts (name, MAC, purpose, serial, OS) VALUES('{newHost[0].hostname}','{newHost[0].MAC}','{newHost[0].purpose}','{newHost[0].serial}','{newHost[0].OS}')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public static void importCSVListHosts(List<host> newHost)
        {
            //06/12/20 to add other DB fields
            //todo 


            for (int i = 0; i < newHost.Count; i++) {

                using (var connectionC = connection()) //uses one subroutine for the connection string builder
                {
                    connectionC.Open();

                    using (var transaction = connectionC.BeginTransaction())
                    {
                        var insertCmd = connectionC.CreateCommand();

                        insertCmd.CommandText = $"INSERT INTO hosts (name, MAC, purpose, serial, OS, Manufacturer, model) VALUES('{newHost[i].hostname}','{newHost[i].MAC}','{newHost[i].purpose}','{newHost[i].serial}','{newHost[i].OS}','{newHost[i].manufacturer}','{newHost[i].model}')";
                        insertCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }

                }
            }
        }

        public static void updateHostDatabase(List<host> host)
        {
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                for (int i = 0; i < host.Count; i++)
                {
                    using (var transaction = connectionC.BeginTransaction())
                    {
                        var insertCmd = connectionC.CreateCommand();
                        insertCmd.CommandText = $"UPDATE hosts SET lastIP = '{host[i].lastIP}', online = {Convert.ToInt32(host[i].online)}, response = {host[i].responseMS}  WHERE name = '{host[i].hostname}';";
                        insertCmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
     }
        public static void incrementSeenCount(List<host> host)
        {
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                for (int i = 0; i < host.Count; i++)
                {
                    if (host[i].online == true) //if the host is marked as online increment the count for timesSeen
                    {
                        using (var transaction = connectionC.BeginTransaction())
                        {
                            var insertCmd = connectionC.CreateCommand();
                            insertCmd.CommandText = $"UPDATE hosts SET timesSeen = timesSeen + 1 WHERE name = '{host[i].hostname}';";
                            insertCmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                }
            }
        }
        public static void setLastdatestamp(List<host> host)
        {
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                for (int i = 0; i < host.Count; i++)
                {
                    if (host[i].online == true) //if the host is marked as online increment the count for timesSeen

                    {
                        //lastLiveTime
                        using (var transaction = connectionC.BeginTransaction())
                        {
                            var insertCmd = connectionC.CreateCommand();
                            insertCmd.CommandText = $"UPDATE hosts SET lastLiveTime = DATE('now') WHERE name = '{host[i].hostname}';";
                            insertCmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    if (host[i].online == false)
                    {
                        break;
                    }
                }
            }
        }

        public static void updateDatabaseEntry(host host) //sends a single host not a list.
        {
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                
                        //todo: 11/1/20 add some exception handling
                        using (var transaction = connectionC.BeginTransaction())
                        {
                            var insertCmd = connectionC.CreateCommand();
                            insertCmd.CommandText = $"UPDATE hosts SET purpose = '{host.purpose}', Manufacturer = '{host.manufacturer}', " +
                        $"serial = '{host.serial}', warranty = '{host.warranty}', Model = '{host.model}', MAC = '{host.MAC}', OS = '{host.OS}'  " +
                        $"WHERE name = '{host.hostname}';";
                            insertCmd.ExecuteNonQuery();
                            transaction.Commit();
                        }

            }
        }

        public static void updateHostsTableActivetickets() //sends a single host not a list.
        {
            //updates the hosts table to set value to match tickets table value. Doesn't work as it doesn't explicitly search for most recent. To remove 07/05/21
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();


                using (var transaction = connectionC.BeginTransaction())
                {
                    var insertCmd = connectionC.CreateCommand();
                    insertCmd.CommandText = $"UPDATE hosts SET activeticket = (SELECT Active FROM tickets WHERE tickets.Hostname = hosts.name);";
                    //insertCmd.CommandText = $"UPDATE hosts SET activeticket = ifnull((SELECT Active FROM tickets WHERE tickets.Hostname = hosts.name),0);";
                    insertCmd.ExecuteNonQuery();
                    transaction.Commit();
                }

            }
        }



        public static List<host> readHostsNamesOnlyFromDatabase(JNetcheckerWindow j)
        {
            List<host> DBhosts = new List<host>();
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            using (var connectionC = connection()) //uses one subroutine for the connection string  builder
            {
                connectionC.Open();

                var selectCmd = connectionC.CreateCommand();

                selectCmd.CommandText = "SELECT name FROM hosts";


                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = reader.GetString(0);
                        string hostName = (string)reader["name"];

                        DBhosts.Add(new host() { hostname = hostName});
                    }
                }
                connectionC.Close();

            }
            return DBhosts;
        }


        public static List<host> readHostsFromDatabase(JNetcheckerWindow j)
        {
            List<host> DBhosts = new List<host>();
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();
                var selectCmd = connectionC.CreateCommand();
                selectCmd.CommandText = "SELECT * FROM hosts ORDER BY name";
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = reader.GetString(0);
                        string hostName = (string)reader["name"];
                        string purpose = (string)reader["purpose"];
                        string serial = (string)reader["serial"];
                        string MAC = (string)reader["MAC"];
                        string lastIP = (string)reader["lastIP"];
                        string OS = (string)reader["OS"];
                        bool online = Convert.ToBoolean((long)reader["online"]);
                        bool ticketactive = Convert.ToBoolean((long)reader["activeticket"]);

                        string warranty = (string)reader["warranty"]; //todo: 09/11/20 build functions to do with dates
                        int timesSeen = Convert.ToInt32((long)reader["timesSeen"]);
                        int response = Convert.ToInt32((long)reader["response"]);

                        //   string time = (string)(reader["lastLiveTime"]);
                        string timeStamp    = (string)(reader["lastLiveTime"]);
                        //string manufacturer = "unknown";
                        string model        = (string)(reader["Model"]);
                        MAC = BlankValueCheck(MAC); //runs the subroutine to check value is not blank, if so displays as Unknown value
                       string manufacturer = BlankValueCheck((string)reader["Manufacturer"]);
                        model = BlankValueCheck(model);
                        serial = BlankValueCheck(serial);
                        OS = BlankValueCheck(OS);
                        purpose = BlankValueCheck(purpose);
                        warranty = BlankValueCheck(warranty);


                        DBhosts.Add(new host() { hostname = hostName, lastIP = lastIP, timesSeen = timesSeen, OS = OS, MAC = MAC, purpose = purpose, online = online, responseMS = response, lastLiveTime= timeStamp, serial=serial, manufacturer=manufacturer,model=model,warranty=warranty, ticketactive= ticketactive });
                    }
                }
                connectionC.Close();

            }
            return DBhosts;
        }
        public static string BlankValueCheck(string value)
        {
            if (value.Length < 1)
            {
                return "Unknown value";
            }
            return value;
            
        }

        public static void deleteHost(string hostname) //sends a single host not a list.
        {
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();

                //todo: 11/1/20 add some exception handling
                using (var transaction = connectionC.BeginTransaction())
                {
                    var insertCmd = connectionC.CreateCommand();

                    insertCmd.CommandText = $"DELETE FROM hosts WHERE name ='{hostname}' ";
                    insertCmd.ExecuteNonQuery();
                    transaction.Commit();
                }

            }
        }



        public static DateTime ConvertToDateTime(string str)
        {
            string pattern = @"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})\.(\d{3})";
            if (Regex.IsMatch(str, pattern))
            {
                Match match = Regex.Match(str, pattern);
                int year = Convert.ToInt32(match.Groups[1].Value);
                int month = Convert.ToInt32(match.Groups[2].Value);
                int day = Convert.ToInt32(match.Groups[3].Value);
                int hour = Convert.ToInt32(match.Groups[4].Value);
                int minute = Convert.ToInt32(match.Groups[5].Value);
                int second = Convert.ToInt32(match.Groups[6].Value);
                int millisecond = Convert.ToInt32(match.Groups[7].Value);
                return new DateTime(year, month, day, hour, minute, second, millisecond);
            }
            else
            {
                throw new Exception("Unable to parse.");
            }
        }

        public static void InitializeDatabase(JNetcheckerWindow j)
        {
            //todo: 07/05/21 initilize the tickets table too, add activeticket bool for hosts
          List<host> DBhosts = new List<host>();
            using (var connectionC = connection()) //uses one subroutine for the connection string builder
            {
                connectionC.Open();

                //Create a table (drop if already exists first):
             var delTableCmd = connectionC.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS hosts";
               delTableCmd.ExecuteNonQuery();

               var createTableCmd = connectionC.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE hosts(name VARCHAR(20) PRIMARY KEY, lastLiveTime DATETIME DEFAULT CURRENT_TIMESTAMP, MAC VARCHAR(17) DEFAULT '', lastIP VARCHAR(39) DEFAULT '', " +
                  "timesSeen int DEFAULT 0, purpose VARCHAR (100) DEFAULT '',serial VARCHAR (100) DEFAULT '', OS varchar(50) DEFAULT '', Manufacturer VARCHAR(50) DEFAULT '', Model VARCHAR (50) DEFAULT '',warranty VARCHAR(50) DEFAULT '', online int DEFAULT 0, response int DEFAULT 0)"; // I believe Windows max length 15.
                createTableCmd.ExecuteNonQuery();

                //Seed some data:
                using (var transaction = connectionC.BeginTransaction())
                {
                    var insertCmd = connectionC.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO hosts (name, purpose) VALUES('raspberrypi','Mainframe cooling')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }


            }
        }
    }
}
 