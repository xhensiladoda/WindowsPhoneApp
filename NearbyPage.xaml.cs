﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Device.Location;
using Windows.Devices.Geolocation;
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
            attesa_txt.Text = "Attendere!\nSto ottenendo la tua posizione GPS!";
            progressbar.Visibility = System.Windows.Visibility.Visible;
            progressbar.IsIndeterminate = true;

            // come default metto un raggio senza limite
            float radius = 100000f;
            // in base alla scelta dell'utente nel ListPicker cambio il valore del raggio
            switch (listraggi.SelectedIndex)
            {
                case 0: radius = 100000f; break;
                case 1: radius = 0.5f; break;
                case 2: radius = 1f; break;
                case 3: radius = 5f; break;
                case 4: radius = 20f; break;
            }

            // invoco la funzione di GeoLocator per ritornare la mia posizione
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            string currentLocation = myGeoposition.Coordinate.ToGeoCoordinate().ToString();

            // estrae le due stringhe con le due posizioni
            string[] coordinate = currentLocation.Split(new char[] { ',' });
            // converte le singole stringe in float
            float latitude = float.Parse(coordinate[0], CultureInfo.InvariantCulture);
            float longitude = float.Parse(coordinate[1], CultureInfo.InvariantCulture);

            // chiamo la funzione get_nearby per ottenere la lista dei PoI vicini 
            // e mando il risultato alla pagina dei risultati
            string result = await WebAPI.get_nearby("1", "1", latitude, longitude, radius);

            progressbar.IsIndeterminate = false;
            progressbar.Visibility = Visibility.Collapsed;

            NavigationService.Navigate(new Uri("/NearbyResultsPage.xaml?result=" + result, UriKind.Relative));
        }
    }
}