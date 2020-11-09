using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace JNetchecker
{
    public static class Config
    {
        public static string rootFolder = @"C:\Temp\commandcentre\";
        //Default file. MAKE SURE TO CHANGE THIS LOCATION AND FILE PATH TO YOUR FILE   
        static public string textFile = @"C:\Temp\commandcentre\commandcentre.txt";
        public static Dictionary<string, bool> configValuesType = new Dictionary<string, bool>(); //janky method of marking the expected value as int or char. TRUE = INT, FALSE =CHAR
        public static Dictionary<string, string> configValues = new Dictionary<string, string>();
        //  List<host> hosts = new List<host>();

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
        public static void testConfigModule(List<host> hosts)
        {
            //default vaulues
        //    hosts[1].timesSeen = 69;
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
    }
}

