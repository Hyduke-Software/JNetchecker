using System;
using System.Collections.Generic;
using System.Text;

namespace JNetchecker
{
    public class TicketList 
    {
        public string Hostname { get; set; }
        public Int64 ID { get; set; }
        public string Poster { get; set; }
        public string Text { get; set; }
        public string Timestamp { get; set; }
        public Int64 Active { get; set; } //would be a bool if SQLite supported them


    }
}