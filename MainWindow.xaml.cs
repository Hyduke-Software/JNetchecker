using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.NetworkInformation;
namespace WpfTutorialSamples.DataGrid_control //todo refactor into my own name
{
    public partial class JNetchecker : Window
    {
        List<host> hosts = new List<host>();
        public JNetchecker()
        {
            InitializeComponent();


            /*   hosts.Add(new host() { Id = 1, hostname = "raspberrypi", lastLiveTime = new DateTime(2020, 1, 1), MAC="" ,lastIP = "", timeseen=0});
               hosts.Add(new host() { Id = 2, hostname = "DiceCam", lastLiveTime = new DateTime(2020, 1, 1), MAC = "", lastIP = "", timeseen = 0 });
               hosts.Add(new host() { Id = 3, hostname = "Emerald-III", lastLiveTime = new DateTime(2020, 1, 1), MAC = "", lastIP = "", timeseen = 0 });
               hosts.Add(new host() { Id = 4, hostname = "hyduke-webserver", lastLiveTime = new DateTime(2020, 1, 1), MAC = "", lastIP = "", timeseen = 0 });
               hosts.Add(new host() { Id = 5, hostname = "zimbabweServer", lastLiveTime = new DateTime(2020, 1, 1), MAC = "", lastIP = "", timeseen = 0 });
               */
            Config.getHostNames(hosts); //gets the values from the text file


            refreshTable(); //sets the datagrid values from the users list.
        }


        public void refreshTable()
        {

            dgSimple.ItemsSource = null;
            dgSimple.ItemsSource = hosts;
        }
        public void Button_Click(object sender, EventArgs e)
        {
            //todo for length
            pinger(0);
            pinger(1);
            pinger(2);
      

        }

        public void pinger(int id) { 
        Ping p = new Ping();
        PingReply r;
        string s;
        s = hosts[id].hostname;
           
            try
            {
                r = p.Send(s);
                _ = r.Status == IPStatus.Success;

                textbox.Text = "Ping to " + s.ToString() + "[" + r.Address.ToString() + "]" + " Successful"
    + " Response delay = " + r.RoundtripTime.ToString() + " ms" + "\n";
                hosts[id].lastLiveTime = DateTime.Now;
                hosts[id].lastIP = r.Address.ToString();
                hosts[id].timeseen = hosts[id].timeseen + 1;
                hosts[id].online = true;

                refreshTable();
    }

            catch (Exception j)
            {
                hosts[id].MAC = "OFFLINE";
                textbox.Text = "" + j;
                hosts[id].online = false;

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Config.testConfigModule(hosts);
            refreshTable();
        }
    }
}

public class host
    {
        public int Id { get; set; }
        public string hostname { get; set; }
        public DateTime lastLiveTime { get; set; }
        public string MAC { get; set; }
        public string lastIP { get; set; }
        public int timeseen { get; set; }
        public bool online { get; set; }
}




