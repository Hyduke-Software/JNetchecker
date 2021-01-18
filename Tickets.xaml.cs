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
        public Tickets(string hostName)
        {
            InitializeComponent();

            TimestampBox.Text = DateTime.Now.ToString();
            hostNameBlock.Text = hostName;
           /// List <TicketList> tickets = new List<TicketList>();


            // List<string[]> ticketResultsList = new List<string[]>(); when it used list
           // tickets = DataAccess.readTickets(hostNameBlock.Text);
            PrintTickets(DataAccess.readTickets(hostName));




        }

        public void PrintTickets(List<TicketList> tickets)
        {
        //prints previous tickets for this hostname.

            foreach (TicketList ticket in tickets)
            {

                previousTicketBox.Text += $"\rPost {ticket.ID}, on {ticket.Timestamp} by {ticket.Poster}:";
                previousTicketBox.Text += $"\r{ticket.Text}\n";

            }




        }

        private void Button_ClickStoreTicket(object sender, RoutedEventArgs e)
        {
            //passes values to subroutine to store in database

            String newTicketTextBoxCleaned = newTicketTextBox.Text;
            newTicketTextBoxCleaned = newTicketTextBoxCleaned.Replace("'", "''"); //adds extra speechmark as SQlite expects two if one is used.
            String poseterBoxTextBoxCleaned = posterNameBox.Text;
            poseterBoxTextBoxCleaned = poseterBoxTextBoxCleaned.Replace("'", "''"); //adds extra speechmark as SQlite expects two if one is used.


            DataAccess.storeTicket(hostNameBlock.Text, newTicketTextBoxCleaned, poseterBoxTextBoxCleaned);

            Close(); //todo add refresh

        }
    }
}
