using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

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
            //todo 23/01/21 remove if unused/duplicate
           dgSimple.ItemsSource = null;
           hosts = Config.getHostNames();
           dgSimple.ItemsSource = hosts;
        }
        public void refreshTable()
        {
            dgSimple.ItemsSource = null;
            hosts = DataAccess.readHostsFromDatabase(this);
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

        private void NewHost_ButtonClick(object sender, RoutedEventArgs e)
        {
            //opens the New Host window
            NewHost nh = new NewHost();
            nh.Show(); //shows window
            nh.Activate(); //moves to foreground

        }

        private void InitilizeDatabaseButtonClick(object sender, RoutedEventArgs e)
        {
             DataAccess.InitializeDatabase(this); //delete. recreate database
            refreshTable();
        }

        private void EditHosts_ButtonClick(object sender, RoutedEventArgs e)
        {
            //opens an editdata window
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

        private void ImportCSVButtonClick(object sender, RoutedEventArgs e)
        {
            //imports the csv file and stores in DB
         //todo: 18/12/20 update logic
            List<host> importedHosts = new List<host>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //gives file explorer window tofind the file to import
            if (openFileDialog.ShowDialog() == true)
            {

        importedHosts = ImportCSV.loadCsvFile(openFileDialog.FileName); //should get file path from dialogue box, send to loadCsvFile subroutine.
                                                                        //searchList is populated with CSV values from file, 19/12/20 changed to use file explorer dialogue box

               // DataAccess.importCSVListHosts(importedHosts); //adds to DB //06/12/20 works with the single value in the list.
                DataAccess.importCSVListHosts(ImportCSV.loadCsvFile(openFileDialog.FileName)); //get file path from openFileDialog, load with loadCsvFile then import with importCSVListHosts() todo: 19/12/20 change logic so that value is passed directly from 
                 //loadCSVfile to importcsvlisthosts within the DataAccess class
            }
            //todo: error action

        }

        private void TicketsButtonClick(object sender, RoutedEventArgs e)
        {
            if (dgSimple.SelectedIndex > -1)
                //requires a row to be selected
            {
                Tickets ViewTicket = new Tickets(hosts[dgSimple.SelectedIndex].hostname); //creates new ViewTicket window for selected host
                ViewTicket.Show();
                return;
            }

            MessageBox.Show("Error 4: Select a host");

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




