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
    /**
    * Controller del form dei PoI vicini
    * @author Francesco Garutti
    */
    public partial class NearbyPage : PhoneApplicationPage
    {
        /**
         * Funzione che crea la lista dei raggi per il ListPicker
         */ 
        public void InitListaRaggi()
        {
            String[] raggi = { "Qualsiasi", "500 m", "1 km", "5 km", "20 km"};
            listraggi.ItemsSource = raggi;
        }

        public NearbyPage()
        {
            InitializeComponent();
            // inizializzo la lista dei raggi
            InitListaRaggi();
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: al momento metto brutalmente latitudine e longitudine anche se in realtà
            // vanno poi prese con il GetLocation()
            float latitude = 44.67f;
            float longitude = 10.97f;
            // come default metto un raggio senza limite
            float radius = 10000f;

            // in base alla scelta dell'utente nel ListPicker cambio il valore del raggio
            switch (listraggi.SelectedIndex)
            {
                case 0: radius = 10000f; break;
                case 1: radius = 0.5f; break;
                case 2: radius = 1f; break;
                case 3: radius = 5f; break;
                case 4: radius = 20f; break;
            }

            // chiamo la funzione get_nearby per ottenere la lista dei PoI vicini 
            // e mando il risultato alla pagina dei risultati
            string result = await WebAPI.get_nearby("1", "1", latitude, longitude, radius);
            NavigationService.Navigate(new Uri("/NearbyResultsPage.xaml?pos="+latitude+"-"+longitude+"&result=" + result, UriKind.Relative));
        }
    }
}