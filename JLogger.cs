using System;
using System.IO;
using System.Threading.Tasks;

namespace JNetchecker
{
    class JLogger
    {
        public static async Task WriteGenericLogFile(string logText)
        {
        //writes a new log entry into the logfile specified in the config JSON file, with a timestamp
            string filepath = Config.readConfig().LoggerFilepath;
            using (StreamWriter file = new StreamWriter(filepath, true))
            {
                await file.WriteLineAsync((DateTime.Now) + "\t" + logText);
            }

        }

    }
}