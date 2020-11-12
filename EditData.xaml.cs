using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.ComponentModel;

namespace JNetchecker
{
    /// <summary>
    /// Interaction logic for EditData.xaml
    /// </summary>
    public partial class EditData : Window
    //https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.ieditableobject?view=netcore-3.1
    {
        public host backupData;
        JNetcheckerWindow j = null;
        List<host> hosts = new List<host>();
        public EditData()
        {
            InitializeComponent();
        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            hosts = DataAccess.readHostsFromDatabase(j); //gets data into the hosts variable
                                                      
            List<host> hostResult = new List<host>();

            /*   var result = from i in hosts
                            where i.hostname == searchbox.Text
                            select i;
                            */

            //todo: 10/11/20 add some error checking!!!!
            hostnameLabel.Content = searchbox.Text;
            foreach (host element in hosts)
            {
                if (element.hostname == searchbox.Text)
                {
                    //todo: 10/11/20 make this copy the value of the element in one swoop
                    hostResult.Add(new host() {hostname=element.hostname,lastIP=element.lastIP,timesSeen=element.timesSeen,
                        OS=element.OS,MAC = element.MAC,purpose = element.purpose, online = element.online,responseMS = element.responseMS, lastLiveTime = element.lastLiveTime 
                    });
           // DBhosts.Add(new host() { hostname = hostName, lastIP = lastIP, timesSeen = timesSeen, OS = OS, MAC = MAC, purpose = purpose, online = online, responseMS = response, lastLiveTime = timeStamp });

                    break; // If you only want to find the first instance a break here would be best for your application
                }
            }


            osBox.Text           = hostResult[0].OS;
            purposeBox.Text      = hostResult[0].purpose;
            MACBox.Text          = hostResult[0].MAC;
            manufacturerBox.Text = "JamesTech";
            modelBox.Text        = "Computer 2000";
            serialBox.Text       = hostResult[0].serial;
            warrantyBox.Text     = hostResult[0].warranty;
            //  
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Update_Host_ButtonClick(object sender, RoutedEventArgs e)
        {
            //creates new host from the text box values
            //passes to function to update database

            if(searchbox.Text == "")
            {
                MessageBox.Show("Enter a hostname");
                return;
            }

            //shouldn't need all the values as it is only updating those that are static. Example: not IP address and last time online.

            DataAccess.updateDatabaseEntry(
                new host()
                {
                    hostname = searchbox.Text,
                    OS = osBox.Text,
                    MAC = MACBox.Text,
                    purpose = purposeBox.Text,
                    warranty = purposeBox.Text,
                    serial = serialBox.Text,
                    manufacturer = manufacturerBox.Text,
                    model = modelBox.Text
                });

            Close();
        }
    }
}
