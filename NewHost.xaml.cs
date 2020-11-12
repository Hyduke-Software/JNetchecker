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

namespace JNetchecker
{
    /// <summary>
    /// Interaction logic for NewHost.xaml
    /// </summary>
    public partial class NewHost : Window
    {
        public NewHost()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //todo: add checking
            List<host> newHostsToSend = new List<host>();

            if (newHostnameEntryBox.Text.Length == 0)
            {
                MessageBox.Show("ERROR 4: New host must have a name.");
            }

            //newHostnameEntryBox
            //newMACEntryBox
            //newPurposeEntryBox
            // newOperatingSystemEntryBox
            // newSerialEntryBox
            //todo: add the rest of value boxes
            if (newHostnameEntryBox.Text.Length > 0)
            {
                newHostsToSend.Add(new host() { hostname = newHostnameEntryBox.Text, MAC= newMACEntryBox.Text, purpose=newPurposeEntryBox.Text, OS=newOperatingSystemEntryBox.Text, serial=newSerialEntryBox.Text});

                DataAccess.addHostToDatabase(newHostsToSend);

            }

            Close();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Close();
        }
    }
}
