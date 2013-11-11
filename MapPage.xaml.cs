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
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Shapes;
using System.Windows.Media;
using sdkMapControlWP8CS;
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
        MapLayer locationLayer = null;

        public MapPage()
        {
            InitializeComponent();
            // Set default zoom level
            poiMap.ZoomLevel = BASE_ZOOMLEVEL;
            // Create the localized ApplicationBar.
            BuildLocalizedApplicationBar();
            // Get current location and center on map
            InitLocation ();
        }

        /** Asyncronus call to initialize the location layer. */
        private async void InitLocation()
        {
            // Get current location
            await GetLocation();
            // Center map on location
            CenterMapOnLocation();
            // Show dot on location
            ShowLocation();
        }

        /** Asyncronus call to get the current location coordinates. */
        private async Task GetLocation()
        {
            // Get current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            currentLocation = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
        }

        /** Paints a circle on the location layer to display the current location of the device. */
        private void ShowLocation()
        {
            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = currentLocation;

            // Create a MapLayer to contain the MapOverlay.
            locationLayer = new MapLayer();
            locationLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            poiMap.Layers.Add(locationLayer);

        }

        /** Centers on the current location. */
        private void CenterMapOnLocation()
        {
            poiMap.Center = currentLocation;
        }

        #region Event handlers for App Bar buttons and menu items
        void ToggleLocation(object sender, EventArgs e)
        {
            ShowLocation();
            CenterMapOnLocation();
        }
        #endregion

        /** Creates the application bar with its buttons and sets the events handlers. */
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