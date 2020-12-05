using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace JNetchecker //todo refactor into my own name
{
    public partial class JNetcheckerWindow : Window
    {
        List<host> hosts = new List<host>();
        public JNetcheckerWindow()
        {
            InitializeComponent();
            refreshTable();
        }

        public void fullRefresh()
        {
           dgSimple.ItemsSource = null;
           hosts = Config.getHostNames();
           dgSimple.ItemsSource = hosts;
        }
        public void refreshTable()
        {
            hosts = DataAccess.readHostsFromDatabase(this);
            dgSimple.ItemsSource = null;
            dgSimple.ItemsSource = hosts;
        }
        public void PingButton_Click(object sender, EventArgs e)
        {
            //calls the pinger() and then uppdates the table with refreshTable()
                Thread t = new Thread(() => pinger());
                t.Start();
            refreshTable();
        }


        public void pinger() { 
        Ping p = new Ping();
        PingReply reply;
        string hostName;
            for (int id = 0; id < hosts.Count; id++)
            {
                try
                {
                    //sends ping command
                    hostName = hosts[id].hostname;
                    reply = p.Send(hostName);
                    _ = reply.Status == IPStatus.Success;

                    hosts[id].lastIP = reply.Address.ToString();
                    hosts[id].online = true;
                    hosts[id].responseMS = Convert.ToInt32(reply.RoundtripTime);
                    Dispatcher.Invoke(() => //This allows the UI to be updated by another thread
                    {
                        textbox.Text = "Ping to " + hostName.ToString() + "[" + reply.Address.ToString() + "]" + " Successful"
               + " Response delay = " + reply.RoundtripTime.ToString() + " ms" + "\n";

                      // 
                    });
                }
                catch (Exception j)
                {
                    Dispatcher.Invoke(() => 
                    { // puts out SQL errors to the textbox somehow, sometimes... 09/11/20
                        textbox.Text = "" + j;
                });

                    //sets host to offline and updates DB
                    hosts[id].online = false;
                    continue; //continues to next 
                }
            }
            //todo 09/11/20 add these into one function
            DataAccess.updateHostDatabase(hosts);
            DataAccess.incrementSeenCount(hosts);
            DataAccess.setLastdatestamp(hosts);
           
        }

        private void refreshDatabaseClick(object sender, RoutedEventArgs e)
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

        private void launchNewHostClick(object sender, RoutedEventArgs e)
        {
            //opens the New Host window
            NewHost nh = new NewHost();
            nh.Show(); //shows window
            nh.Activate(); //moves to foreground

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
             DataAccess.InitializeDatabase(this); //delete. recreate database
            refreshTable();
        }

        private void search_button(object sender, RoutedEventArgs e)
        {
            EditData ed = new EditData();
            ed.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("mstsc", "/v:" + hosts[dgSimple.SelectedIndex].hostname);
                //opens a new MSTSC window going to the server row selected
            }
            catch
            {
                MessageBox.Show("Error 5: No host selected in the table.");

            }
        }
        private void about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A lightweight asset manager with monitoring tools developed by Hyduke Software. \n\nSupport: assetmanager@hyduke-software.net. \nCopyright 2020", "J Asset Manager Light");
        }
        private void launchPowerShell(object sender, RoutedEventArgs e)
        {
           // string strCmdText = Path.Combine(Directory.GetCurrentDirectory(), "testscript.ps1"); todo use var instead of hardcoded 

            Process.Start("powershell", "C:/Users/James/Desktop/testscript.ps1 emerald-iii");
            //test to launch a powershell script with a variable. TODO: 14/11/20 add a function such as 

        }

    }
}

public class host
    {
    public string hostname { get; set; }
    public bool online { get; set; }
    //public DateTime lastLiveTime { get; set; }
    public string lastLiveTime { get; set; }

    public string lastIP { get; set; }
    public int responseMS { get; set; }
    public int timesSeen{ get; set; }

    public string purpose { get; set; }
    public string OS { get; set; }
    public string MAC { get; set; }
    public string serial { get; set; }
    //todo 11/11/20 make warranty a date
    public string warranty { get; set; }
    public string manufacturer { get; set; }
    public string model { get; set; }
    //new values for later release 14/11/20
    public string location { get; set; }
    public string owner { get; set; }
    public string notes { get; set; }


}




