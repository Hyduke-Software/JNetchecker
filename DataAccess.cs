
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;


namespace JNetchecker
{
    public class DataAccess
    {

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

        public static void addHostToDatabase(List<host> newHost)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            //   connectionStringBuilder.DataSource = "./SqliteDB.db";
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = $"INSERT INTO hosts (name, MAC, purpose, serial, OS) VALUES('{newHost[0].hostname}','{newHost[0].MAC}','{newHost[0].purpose}','{newHost[0].serial}','{newHost[0].OS}')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public static void updateHostDatabase(List<host> host)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            //   connectionStringBuilder.DataSource = "./SqliteDB.db";
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();


                for (int i = 0; i < host.Count; i++)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        var insertCmd = connection.CreateCommand();
                        //UPDATE Customers
                        //SET ContactName = 'Juan'
                        //WHERE Country = 'Mexico';
                        insertCmd.CommandText = $"UPDATE hosts SET lastIP = '{host[i].lastIP}', online = {Convert.ToInt32(host[i].online)}  WHERE name = '{host[i].hostname}';";

                        insertCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }

            }
     }


        public static void incrementSeenCount(List<host> host)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                for (int i = 0; i < host.Count; i++)
                {
                    if (host[i].online == true) //if the host is marked as online increment the count for timesSeen
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var insertCmd = connection.CreateCommand();
                            insertCmd.CommandText = $"UPDATE hosts SET timesSeen = timesSeen + 1 WHERE name = '{host[i].hostname}';";
                            insertCmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }
                }

            }
        }



        public static List<host> readHostsFromDatabase(JNetcheckerWindow j)
        {
            List<host> DBhosts = new List<host>();
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            //   connectionStringBuilder.DataSource = "./SqliteDB.db";
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
               
                selectCmd.CommandText = "SELECT * FROM hosts";
                

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = reader.GetString(0);
                        string hostName = (string)reader["name"];
                        string lastLiveTime = ""; 
                        //I don't know if this works as I don't have any values
                        try
                        {
                            if ((string)reader["lastLiveTime"] !=null)
                            {
                                lastLiveTime = (string)reader["lastLiveTime"].ToString();
                            }
                        }
                        catch(Exception)
                        {
                        }

                        //  string lastLiveTime = (string)reader["lastLiveTime"] ?? "never"; //todo: 09/11/20 add functions to manage dates when no date type
                      
                        string purpose = (string)reader["purpose"];
                        string serial = (string)reader["serial"] ?? "unknown"; //the ?? doesn't work
                        string MAC = (string)reader["MAC"] ?? "not known";
                        string lastIP = (string)reader["lastIP"] ?? "tba";
                        string OS = (string)reader["OS"] ?? "not known";
                        //  bool online = Convert.ToBoolean(Convert.ToInt32((long)reader["online"])); //as SQLite has no bool type

                       int temp = Convert.ToInt32((long)reader["online"]);
                        bool online = Convert.ToBoolean(temp);

                        string warranty = (string)reader["warranty"] ?? "not know"; //todo: 09/11/20 build functions to do with dates

                        int timesSeen = Convert.ToInt32((long)reader["timesSeen"]);
                        DBhosts.Add(new host() { hostname = hostName, lastIP = lastIP, timesSeen = timesSeen, OS= OS, MAC=MAC, purpose=purpose, online=online});
                        // DBhosts.Add(new host() { hostname = hostName, lastIP = lastIP, timesSeen = timesSeen, OS= OS, MAC=MAC, serial=serial});

                    }
                    //   j.getNewHostsList(DBhosts);
                }
                connection.Close();

            }
            return DBhosts;
        }


        public static void InitializeDatabase(JNetcheckerWindow j)
        {
          List<host> DBhosts = new List<host>();
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            //   connectionStringBuilder.DataSource = "./SqliteDB.db";
            connectionStringBuilder.DataSource = "C:/Temp/commandcentre/SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Create a table (drop if already exists first):
             var delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS hosts";
               delTableCmd.ExecuteNonQuery();

               var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE hosts(name VARCHAR(20) PRIMARY KEY, lastLiveTime VARCHAR(30) DEFAULT '', MAC VARCHAR(17) DEFAULT '', lastIP VARCHAR(39) DEFAULT '', " +
                  "timesSeen int DEFAULT 0, purpose VARCHAR (100) DEFAULT '',serial VARCHAR (100) DEFAULT '', OS varchar(50) DEFAULT '', Manufacturer VARCHAR(50), Model VARCHAR (50) DEFAULT '',warranty VARCHAR(50) DEFAULT '', online int DEFAULT 0)"; // I believe Windows max length 15.
                createTableCmd.ExecuteNonQuery();

                //Seed some data:
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = "INSERT INTO hosts (name, purpose) VALUES('raspberrypi','Mainframe cooling')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "INSERT INTO hosts (name, purpose) VALUES('emerald-iii','owning noobs')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "INSERT INTO hosts (name, purpose) VALUES('192.168.1.254','The Internets')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }


            }
        }
    }
}
 