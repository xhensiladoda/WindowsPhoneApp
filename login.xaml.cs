﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace AppSDEM
{  
    /**
    * Controller responsabile del login.
    * @author Doda Xhensila
    */
    public partial class login : PhoneApplicationPage
    {
        public login()
        {
            InitializeComponent();
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {   
            //recupero i dati dai textbox
            TextBox user_t = (TextBox)user;
            String username = user_t.Text;
            TextBox password_t = (TextBox)pass;
            String password = password_t.Text;
            GetId(username, password);
        }
        /**
         * Funzione responsabile della chiamata per il login, che viene effettuata
         * in base ai parametri inseriti dall'utente nei textbox.
         */
        private async void GetId(String username,String password) {
            string json = await WebAPI.login("1", username, password);
            List<User> user = new List<User>();
            user = Utils.DeserializeJSONArray<User>(json);
            if (user[0].pk != 0)
            {
                // salvo userPK e userName in cache in modo da ottenerli in avvii successivi
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("userPK"))
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("userPK", user[0].pk);
                    IsolatedStorageSettings.ApplicationSettings.Add("userName", user[0].fields.first_name);
                }

                IsolatedStorageSettings.ApplicationSettings["userPK"] = user[0].pk;
                IsolatedStorageSettings.ApplicationSettings["userName"] = user[0].fields.first_name;
                IsolatedStorageSettings.ApplicationSettings.Save();

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else 
                MessageBox.Show("Username o Password errati");
        }
    }
}