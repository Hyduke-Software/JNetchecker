using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace JNetchecker
{
    public class ImportCSV
    {
        //   public static List<string> loadCsvFile(string filePath)
        public static List<host> loadCsvFile(string filePath)
        {
            //imports the csv file as a list
                var reader = new StreamReader(File.OpenRead(filePath));
                List<string> hostListfromCSV = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    hostListfromCSV.Add(line);
                }
            // return hostListfromCSV;
            return convertCSVStringstoHostsList(hostListfromCSV);
            // 18/12/20 if this is right it will call the convertCSVStringstoHostsList and return the value form that.


            //  verifyCSV(hostListfromCSV);
        }


        public static List<host> verifyCSV(string hostFromCSV)
        {
            //todo: make methods to ensure data from CSV is valid. 05/12/20
            //make it take a string array and convert each line into a host in a hostlist

            String[] extractedStrings = hostFromCSV.Split(',');
            ///todo: count how  many values per line by number of commas
            ///

           
            if (extractedStrings.Length > 11)
            {
                //to test: 06/12/20
                MessageBox.Show("extracted string lentgh is invalid" + extractedStrings.Length);
                return null;

            }

            //importedHost.hostname = extractedStrings[0];
            host importedHost = new host();
            List<host> importListhosts = new List<host>();
            importListhosts.Add(new host()
            {
                hostname        = extractedStrings[0],
                purpose         = extractedStrings[1],
                OS              = extractedStrings[2],
                MAC             = extractedStrings[3],
                serial          = extractedStrings[4],
                warranty        = extractedStrings[5],
                manufacturer    = extractedStrings[6],
                model           = extractedStrings[7],
                location        = extractedStrings[8],
                owner           = extractedStrings[9],
                notes           = extractedStrings[10]

            }); ;

            MessageBox.Show("extrateced hostname: " +extractedStrings[0] +"\t"+ extractedStrings[1] + "\t" + extractedStrings[2] + "\t" + extractedStrings[3] + "\t" + extractedStrings[4] + "\t" + extractedStrings[5] + "\t" + extractedStrings[6] + "\t" + extractedStrings[7] + "\t" + extractedStrings[8] + "\t" + extractedStrings[9]
                + "\t" + extractedStrings[10]);
            return importListhosts;
        }


        public static List<host> convertCSVStringstoHostsList(List<string> hostFromCSV)
        {
            //todo: make methods to ensure data from CSV is valid. 05/12/20
            //make it take a string array and convert each line into a host in a hostlist


            List<host> importListhosts = new List<host>();
            foreach (string csvLine in hostFromCSV)
            {
                //takes the line from the CSV file, extracts the values seperated by commas.
                String[] extractedStrings = csvLine.Split(',');

                ///todo: count how  many values per line by number of commas 06/12/20



                //importedHost.hostname = extractedStrings[0];
              //  host importedHost = new host();
              
                importListhosts.Add(new host()
                {
                    hostname = extractedStrings[0],
                    purpose = extractedStrings[1],
                    OS = extractedStrings[2],
                    MAC = extractedStrings[3],
                    serial = extractedStrings[4],
                    warranty = extractedStrings[5],
                    manufacturer = extractedStrings[6],
                    model = extractedStrings[7],
                    location = extractedStrings[8],
                    owner = extractedStrings[9],
                    notes = extractedStrings[10]

                }); ;

                MessageBox.Show("extrateced hostname: " + extractedStrings[0] + "\t" + extractedStrings[1] + "\t" + extractedStrings[2] + "\t" + extractedStrings[3] + "\t" + extractedStrings[4] + "\t" + extractedStrings[5] + "\t" + extractedStrings[6] + "\t" + extractedStrings[7] + "\t" + extractedStrings[8] + "\t" + extractedStrings[9]
                    + "\t" + extractedStrings[10]);
            }
            return importListhosts;
        }

    }
    }