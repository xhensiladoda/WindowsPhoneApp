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
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           /* string result = await WebAPI.get_nearby("1", "1", 44.63f, 10.95f, 2.2f);
            //string result = await WebAPI.poi_ids("1");
            Test1.Text = result;*/
            string json = await WebAPI.get_user_checkin("1","9","True");
            List<User> user = new List<User>();
            user = Utils.DeserializeJSONArray<User>(json);
            if (user[0].pk != 0)
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            else MessageBox.Show("Username o Password errati");


            
        }
    }
}