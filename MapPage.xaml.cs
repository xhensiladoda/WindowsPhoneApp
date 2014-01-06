using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading.Tasks;
using AppSDEM.Resources;

namespace AppSDEM
{
    /**
     * Questo controller fornisce le funzionalità della mappa dell'app.
     * Di default la mappa viene centrata sulla posizione corrente del dispositivo.
     * @author Setti Davide
     */
    public partial class MapPage : PhoneApplicationPage
    {
        /** Base zomm level. */
        const int BASE_ZOOMLEVEL = 10;
        /** Coordinates of current location. */
        GeoCoordinate currentLocation = null;
        /** Layer for display current location on the Map. */
        MapLayer poiLayer = null;

        public MapPage()
        {
            InitializeComponent();
            // Set default zoom level
            poiMap.ZoomLevel = BASE_ZOOMLEVEL;
            // Create the localized ApplicationBar.
            BuildLocalizedApplicationBar();
            // Initialize location
            InitLocation();
            PrintPoi();
        }

        /** 
         * Chiamata asincrona per inizializzare la posizione sulla mappa
         */
        private async void InitLocation()
        {
            // Center map on location
            await GetLocation();
            await CenterMapOnLocation();
            // Show dot on location
            ShowLocation();
         }

        /**
         * Chiamata asincrona per ottenere la posizione attuale.
         */
        private async Task GetLocation()
        {
            // Get current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            currentLocation = myGeoposition.Coordinate.ToGeoCoordinate();
        }

        /**
         * Visualizza la posizione attuale del dispositivo.
         */
        private void ShowLocation()
        {
            UserLocationMarker marker = (UserLocationMarker)this.FindName("deviceMarker");
            marker.GeoCoordinate = currentLocation;
            marker.Visibility = Visibility.Visible;
        }

        /**
         * Chiamata asincrona per inserire all'interno della mappa i pushpin
         * dei PoI, ottenuti tramite l'API <code>poi_update</code>.
         */
        private async void PrintPoi()
        {
            // chiamata alla webapi
            string json = await WebAPI.poi_update("2", false);
            List<PoI> poiList = new List<PoI>();
            // recupera la lista dei PoI
            poiList = Utils.DeserializeJSONArray<PoI>(json);
            if (poiList.Count != 0)
            {
                // per ogni poi aggiunge il pushpin alla mappa
                poiLayer = new MapLayer();
                foreach(PoI poi in poiList)
                {
                    Pushpin pushpin = poi.BuildPushpin();
                    MapOverlay myOverlay = new MapOverlay();
                    myOverlay.Content = pushpin;
                    myOverlay.GeoCoordinate = poi.GetCoordinate();
                    poiLayer.Add(myOverlay);
                }
                poiMap.Layers.Add(poiLayer);
            }
        }

        /**
         * Centra la mappa sulla pozione attuale del dispositivo.
         */
        private async Task CenterMapOnLocation()
        {
            await GetLocation();
            poiMap.Center = currentLocation;
        }

        #region Event handlers for App Bar buttons and menu items
        void ToggleLocation(object sender, EventArgs e)
        {
            ShowLocation();
            CenterMapOnLocation();
        }

        void PoiSearch(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PoISearchPage.xaml", UriKind.Relative));
        }

        void GetNearby(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NearbyPage.xaml", UriKind.Relative));
        }
        #endregion

        /**
         * Crea un'ApplicationBar ed imposta i gestori degli eventi.
         */
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;

            // Create buttons with localized strings from AppResources
            // Toggle Location button.
            ApplicationBarIconButton centermapBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/centerme.png", UriKind.Relative));
            centermapBarButton.Text = AppResources.AppBarToggleLocationBT;
            centermapBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(centermapBarButton);

            // creazione dell'icona di Search PoI
            ApplicationBarIconButton searchBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/search.png", UriKind.Relative));
            searchBarButton.Text = "PoI Search";
            searchBarButton.Click += PoiSearch;
            ApplicationBar.Buttons.Add(searchBarButton);

            // creazione dell'icona di Get Nearby
            ApplicationBarIconButton nearbyBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/nearby.png", UriKind.Relative));
            nearbyBarButton.Text = "PoI Vicini";
            nearbyBarButton.Click += GetNearby;
            ApplicationBar.Buttons.Add(nearbyBarButton);

            // Create menu items with localized strings from AppResources.
            // Toggle Location menu item.
            ApplicationBarMenuItem centermapBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLocationMIT);
            centermapBarMenuItem.Click += ToggleLocation;
            ApplicationBar.MenuItems.Add(centermapBarMenuItem);

            // creazione menu item di Search PoI
            ApplicationBarMenuItem searchBarMenuItem = new ApplicationBarMenuItem();
            searchBarMenuItem.Text = "PoI Search";
            searchBarMenuItem.Click += PoiSearch;
            ApplicationBar.MenuItems.Add(searchBarMenuItem);

            // creazione menu item di GetNearby
            ApplicationBarMenuItem nearbyBarMenuItem = new ApplicationBarMenuItem();
            nearbyBarMenuItem.Text = "PoI Vicini";
            nearbyBarMenuItem.Click += GetNearby;
            ApplicationBar.MenuItems.Add(nearbyBarMenuItem);
        }
    }
}