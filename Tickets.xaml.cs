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
    /// Interaction logic for Tickets.xaml
    /// </summary>
    public partial class Tickets : Window
    {
        int active = 0; 
        public Tickets(string hostName)
        {
            InitializeComponent();

            TimestampBox.Text = DateTime.Now.ToString();
            hostNameBlock.Text = hostName;
            posterNameBox.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name; //sets the Poster box to currently logged on user in Windows. Perhaps add feature to make this locked
            /// List <TicketList> tickets = new List<TicketList>();


            // List<string[]> ticketResultsList = new List<string[]>(); when it used list
            // tickets = DataAccess.readTickets(hostNameBlock.Text);

            try { 
            PrintTickets(DataAccess.readTickets(hostName));
            }
            catch (Exception)
            {
                MessageBox.Show("Error 6");

            }
            ///gets list of tickets relating to this host, passes to PrintTickets to update buttons and text boxes
            //todo: 24/01/21 add necessary error handling so that if no ticket exists it does not crash
        }

        public void PrintTickets(List<TicketList> tickets)
        {
            //prints previous tickets for this hostname.


            if (tickets[0].Active == 1)
            {
                activeLabel.Content = "Ticket is active";
                activeLabel.Background = Brushes.DarkSeaGreen;
                //if active field in database is 1 then the ticket is active
                MarkActiveButton.Content = "Mark inactive";
                active = 1;
            }
            if (tickets[0].Active ==0)
            {
                activeLabel.Content = "Ticket is inactive";
                activeLabel.Background = Brushes.DarkOrange;
                //if the active field in database is 0 then it is inactive, button press makes active.
                MarkActiveButton.Content = "Mark active";
                active = 0;
            }

            foreach (TicketList ticket in tickets)
            {
                //concats these new values with existing textbox contents
                previousTicketBox.Text += $"\rPosted {ticket.Timestamp} by {ticket.Poster}:";
                previousTicketBox.Text += $"\r{ticket.Text}\n ----------------------------------------------";
            }

        }

        private void Button_ClickStoreTicket(object sender, RoutedEventArgs e)
        {
            //passes values to subroutine to store in database

            String newTicketTextBoxCleaned = newTicketTextBox.Text;
            newTicketTextBoxCleaned = newTicketTextBoxCleaned.Replace("'", "''"); //adds extra speechmark as SQlite expects two if one is used.
            String poseterBoxTextBoxCleaned = posterNameBox.Text;
            poseterBoxTextBoxCleaned = poseterBoxTextBoxCleaned.Replace("'", "''"); //adds extra speechmark as SQlite expects two if one is used.


            DataAccess.storeTicket(hostNameBlock.Text, newTicketTextBoxCleaned, poseterBoxTextBoxCleaned, active);

            Close(); //todo add refresh

        }

        private void MarkActiveButton_Click(object sender, RoutedEventArgs e)
        {
          if (active == 0)  //if not active. No bools in SQLite
            {
                MessageBox.Show("Will be marked as active upon saving.");
                active = 1;
                MarkActiveButton.Content = "Mark ticket as active";
                activeLabel.Content = "Ticket is active";
                activeLabel.Background = Brushes.DarkSeaGreen;
                return; // return stops the below if from running on the now active = 1
            }

            if (active == 1)  //if active. No bools in SQLite
            {
                MessageBox.Show("Will be marked as inactive upon saving.");
                active = 0;
                MarkActiveButton.Content = "Mark ticket as inactive";
                activeLabel.Content = "Ticket is inactive";
                activeLabel.Background = Brushes.DarkOrange;
                return;
               
            }
        }
    }
}
