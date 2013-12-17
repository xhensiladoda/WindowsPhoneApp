using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace AppSDEM
{
    public partial class DetailPage : PhoneApplicationPage
    {
        public DetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // attiva la proress bar
            progress.IsIndeterminate = true;
            // ottiene i dati del PoI
            string poi_id = NavigationContext.QueryString["poi_id"];
            string json = await WebAPI.poi_details("2", poi_id);
            List<PoI> poiList = new List<PoI>();
            poiList = Utils.deserializeJSONArray<PoI>(json);
            PoI myPoI = poiList[0];
            // imposta i campi della pagina
            shortdescr.Text = myPoI.short_description;
            image.Source = myPoI.GetThumbImage().Source;
            longdescr.Text = myPoI.long_description;
            city.Text = myPoI.city;
            province.Text = myPoI.province;
            zipcode.Text = myPoI.zipcode;
            address.Text = myPoI.address;
            // ferma e nasconde la progress bar
            progress.IsIndeterminate = false;
            progress.Visibility = Visibility.Collapsed;
        }
    }
}
