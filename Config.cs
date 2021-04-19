using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;
namespace JNetchecker
    //Class for saving/reading configuration settings
{
    public static class Config
    {
        public static string rootFolder = @"C:\Temp\commandcentre\";
        //Default file. MAKE SURE TO CHANGE THIS LOCATION AND FILE PATH TO YOUR FILE   
        static public string textFile = @"C:\Temp\commandcentre\commandcentre.txt";
        public static Dictionary<string, bool> configValuesType = new Dictionary<string, bool>(); //janky method of marking the expected value as int or char. TRUE = INT, FALSE =CHAR
        public static Dictionary<string, string> configValues = new Dictionary<string, string>();
        //  List<host> hosts = new List<host>();

 
        public static void testConfigModule(List<host> hosts)
        {
            //default vaulues
        //    hosts[1].timesSeen = 69;
        }
        public static void writeConfig()
        {
            string jsonFilepath = @"c:\temp\netcheck.json";
            string databaseFilepath = SelectDatabaseFile();
            if (databaseFilepath == "MISSING")
            {
                MessageBox.Show("Error 9: Invalid file path provided.");
                return;

            }
            //string filepath = @"c:\temp\netcheck.json";
            ConfigurationValues cv = new ConfigurationValues() ;
            cv.TemperatureCelsius = 69;
            cv.DatabaseFilepath = databaseFilepath;
            // cv.DatabaseFilepath = @"C:\Temp\commandcentre\SqliteDB.db";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            //outputs the JSON file with user friendly indentation
            string jsonString = JsonSerializer.Serialize(cv,options);
            File.WriteAllText(jsonFilepath, jsonString);


        }

       /* public async static ConfigurationValues readConfig()
        {
            An asyncrhornous version of the method.
            ConfigurationValues cv = new ConfigurationValues();
            string filepath = @"c:\temp\netcheck.json";
            using FileStream openStream = File.OpenRead(filepath);
            cv = await JsonSerializer.DeserializeAsync<ConfigurationValues>(openStream);
            MessageBox.Show("Summary: " +cv.Summary +"\n temp: "+cv.TemperatureCelsius);
        }
        */
        public static ConfigurationValues readConfig()
        {
            //19/04/21 reads the configuration file with a hardcoded location
            string filepath = @"c:\temp\netcheck.json";
            string jsonString = File.ReadAllText(filepath);
            ConfigurationValues cv = new ConfigurationValues();
            cv = JsonSerializer.Deserialize<ConfigurationValues>(jsonString);
            return cv;
        }

        public static string SelectDatabaseFile() 
        {
            //gets the filepath for the database
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Database files (*.db)|*.db|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {

                string filepath = (openFileDialog.FileName); //should get file path from dialogue box, send to loadCsvFile subroutine.
                return filepath;
            }
            //todo: error action
            return "MISSING";

        }

        public static List<host> getHostNames()
        {
            List<host> hosts = new List<host>();
            if (File.Exists(textFile))
            {
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);


                for (int i = 0; i < lines.Length; i++)
                {

                    hosts.Add(new host() { hostname = lines[i] });
                    //parts.Add(new Part() {PartName="crank arm", PartId=1234});

                }
                return hosts;

            }
            //todo: return some sort of failure
            return hosts;

            //   ValidateValues();
        }
        public static void addNewHost(string newHostName, List<host> hosts)
        {
            if (File.Exists(textFile))
            {
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);
                using (StreamWriter file =
              new StreamWriter(textFile, true))
                {
                    file.WriteLine(newHostName);
                    //adds new host to end
                }


            }
        }
    }
}
public class ConfigurationValues
{

    public int TemperatureCelsius { get; set; }
    public string DatabaseFilepath { get; set; }
}
