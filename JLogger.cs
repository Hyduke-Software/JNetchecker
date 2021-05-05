using System;
using System.IO;
using System.Threading.Tasks;

namespace JNetchecker
{
    class JLogger
    {
        public static async Task ExampleAsync(string logText)
        {
        //writes a new log entry into the logfile specified in the config JSON file
            string filepath = Config.readConfig().LoggerFilepath;
            using (StreamWriter file = new StreamWriter(filepath, true))
            {
               DateTime sourceTime = DateTime.Now;
                await file.WriteLineAsync(sourceTime + "\t" + logText);
            }

        }

    }
}