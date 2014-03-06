using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AppSDEM.Resources;
using System.IO.IsolatedStorage;

namespace AppSDEM
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Costruttore
        public MainPage()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("userPK"))
            {
                int userPK = (int)IsolatedStorageSettings.ApplicationSettings["userPK"];
                if (userPK == 0)
                {
                    loginbutton.Visibility = System.Windows.Visibility.Visible;
                    logoutbutton.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    loginbutton.Visibility = System.Windows.Visibility.Collapsed;
                    logoutbutton.Visibility = System.Windows.Visibility.Visible;
                    MessageBox.Show("ciao n." + userPK + " -> " + IsolatedStorageSettings.ApplicationSettings["userName"]);
                }
            }
            else
            {
                loginbutton.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/login.xaml", UriKind.Relative));
            //Test2.NavigateUri = new Uri("/Test.xaml", UriKind.Relative);
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/camera.xaml", UriKind.Relative));
        }


        private void checkinbutton_Click(object sender, RoutedEventArgs e)
        {
            if ((!IsolatedStorageSettings.ApplicationSettings.Contains("userPK")) || 
                (int)IsolatedStorageSettings.ApplicationSettings["userPK"] == 0)
            {
                MessageBox.Show("Non puoi visualizzare i Checkin se non sei registrato!");
            }
            else
            {
                NavigationService.Navigate(new Uri("/CheckInResult.xaml", UriKind.Relative));
            }
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["userPK"] = 0;
            IsolatedStorageSettings.ApplicationSettings.Save();
            MessageBox.Show("Ti sei scollegato!!");
            loginbutton.Visibility = System.Windows.Visibility.Visible;
            logoutbutton.Visibility = System.Windows.Visibility.Collapsed;
        }


    }
}