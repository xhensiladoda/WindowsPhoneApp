using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
namespace AppSDEM
{
    public partial class login : PhoneApplicationPage
    {
        public login()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox user_t = (TextBox)user;
            String username = user_t.Text;
            Console.Write("username ");
            Console.WriteLine(username);
            TextBox password_t = (TextBox)pass;
            String password = password_t.Text;
            Console.Write("password ");
            Console.WriteLine(password);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));


        }
    }
}