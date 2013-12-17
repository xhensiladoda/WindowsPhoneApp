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
    /** This controller provides the Map functionality of the App.
     * This is the controller for the Map page of the App.
     * By default the Map is centered on the device current location.
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


        private async void PrintPoi()
        {
            string json = await WebAPI.poi_update("2", false);
            List<PoI> poiList = new List<PoI>();
            poiList = Utils.deserializeJSONArray<PoI>(json);
            if (poiList.Count != 0)
            {
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
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/landmarks.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarToggleLocationBT;
            appBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create menu items with localized strings from AppResources.
            // Toggle Location menu item.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLocationMIT);
            appBarMenuItem.Click += ToggleLocation;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }
    }
}