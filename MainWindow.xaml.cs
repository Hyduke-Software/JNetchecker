using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.NetworkInformation;
using System.Threading;

namespace JNetchecker //todo refactor into my own name
{
    public partial class JNetcheckerWindow : Window
    {
        List<host> hosts = new List<host>();
        //  JNetcheckerWindow james = new JNetcheckerWindow();
        public JNetcheckerWindow()
        {
            InitializeComponent();
           
            // 
            refreshTable();
        }

        public void fullRefresh()
        {
            //dgSimple.ItemsSource = null;
         //   hosts = Config.getHostNames();
            dgSimple.ItemsSource = hosts;
        }
        public void refreshTable()
        {
            hosts = DataAccess.readHostsFromDatabase(this);
            dgSimple.ItemsSource = null;
            dgSimple.ItemsSource = hosts;
        }
        public void Button_Click(object sender, EventArgs e)
        {
                Thread t = new Thread(() => pinger());
                t.Start();
        }


        public void pinger() { 
        Ping p = new Ping();
        PingReply reply;
        string hostName;


            for (int id = 0; id < hosts.Count; id++)
            {
                try
                {
                    hostName = hosts[id].hostname;
                    reply = p.Send(hostName);
                    _ = reply.Status == IPStatus.Success;

               
                    hosts[id].lastLiveTime = DateTime.Now;
                    hosts[id].lastIP = reply.Address.ToString();
                    hosts[id].online = true;
                    hosts[id].responseMS = reply.RoundtripTime.ToString();
                    DataAccess.updateHostDatabase(hosts);
                    DataAccess.incrementSeenCount(hosts);
                    Dispatcher.Invoke(() => //This allows the UI to be updated by another thread
                    {
                        textbox.Text = "Ping to " + hostName.ToString() + "[" + reply.Address.ToString() + "]" + " Successful"
               + " Response delay = " + reply.RoundtripTime.ToString() + " ms" + "\n";

                        refreshTable();
                    });
                }
                catch (Exception j)
                {
                    //hosts[id].MAC = "OFFLINE";


                    Dispatcher.Invoke(() => //This allows the UI to be updated by another thread
                    {
                    //  hosts[id].online = false;
                        textbox.Text = "" + j;

                });


                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           hosts = DataAccess.readHostsFromDatabase(this);
            refreshTable();
        }

        public void textBoxUpdater(string text)
        {

            textbox.AppendText(text+"\r");
        }


        public void getNewHostsList(List<host> newHost)
        {
            hosts = newHost;

            refreshTable();
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            NewHost nh = new NewHost();
            nh.Show();
            nh.Activate();
           //nh.Close();
            fullRefresh();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
             DataAccess.InitializeDatabase(this); //delete. recreate database
            refreshTable();
        }
    }
}

public class host
    {
    public string hostname { get; set; }
    public bool online { get; set; }
    public DateTime lastLiveTime { get; set; }
    
    public string lastIP { get; set; }
    public string responseMS { get; set; }
    public int timesSeen{ get; set; }

    public string purpose { get; set; }
    public string OS { get; set; }
    public string MAC { get; set; }
    public string serial { get; set; }
    public DateTime warranty { get; set; }
    //below are values that must be entered manually in this version






}




