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
        const int BASE_ZOOMLEVEL = 5;
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


        public class PoIonTap {
            public int id { get; set; }
            public string name { get; set; }
            public Image imm { get; set; }
        }

        /**
         * Chiamata asincrona per inserire all'interno della mappa i pushpin
         * dei PoI, ottenuti tramite l'API <code>poi_update</code>.
         */
        private async void PrintPoi()
        {
            // chiamata alla webapi per la lista PoI
            string json = await WebAPI.poi_update("2", false);
            List<PoI> poiList = new List<PoI>();
            // recupera la lista dei PoI
            poiList = Utils.DeserializeJSONArray<PoI>(json);

            // chiamata alla webapi per la lista categorie
            json = await WebAPI.categories_update("1", false);
            List<Categoria> catList = new List<Categoria>();
            // recupera la lista delle categorie
            catList = Utils.DeserializeJSONArray<Categoria>(json);
                
            if (poiList.Count != 0)
            {  
                Image catImage;
                // creo un array di PoIonTap contenente dati che mi servirà passare nel Tap del PoI
                PoIonTap[] poiTap = new PoIonTap[poiList.Count];
                for (int i = 0; i < poiList.Count; ++i)
                {
                    poiTap[i] = new PoIonTap();
                }

                poiLayer = new MapLayer();
                // per ogni poi aggiunge il pushpin alla mappa
                for (int i=0; i<poiList.Count; ++i)
                {
                    MapOverlay myOverlay = new MapOverlay();
                    // estraggo l'immagine della categoria a cui il PoI appartiene
                    catImage = catList[poiList[i].idcategory - 1].GetNormImage();
                    // riempio il poiTap con i dati che mi serve passare nella imm_Tap
                    poiTap[i].id = poiList[i].idpoi;
                    poiTap[i].imm = catImage;
                    poiTap[i].name = poiList[i].short_description;
                    
                    // setto l'immagine della categoria come pushpin
                    myOverlay.Content = poiTap[i].imm;
                    // setto il DataContext da passare nella funzione imm_Tap
                    poiTap[i].imm.DataContext = poiTap[i];
                    poiTap[i].imm.Tap += imm_Tap;
                       
                    myOverlay.GeoCoordinate = poiList[i].GetCoordinate();
                    poiLayer.Add(myOverlay);
                }
                poiMap.Layers.Add(poiLayer);               
            }
        }

        /*
         * Funzione che si attiva al tap di un pushpin
         */ 
        void imm_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // ottengo i parametri che ho passsato e li metto nell'item
            PoIonTap item = ((FrameworkElement)e.OriginalSource).DataContext as PoIonTap;

            // creo l'hyperlink button per passare alla pagina dei dettagli
            HyperlinkButton button = new HyperlinkButton();
            button.Content = "more details >>>";
            // crea la querystring
            string qString = "poi_id=" + item.id;
            // imposto la qString come DataContext che verrà poi passata alla button_Tap
            button.DataContext = qString;
            button.Tap += button_Tap;
            
            // creo una CustomMessaBox per visualizzare il nome del PoI su cui clicco
            CustomMessageBox cmb = new CustomMessageBox()
                {
                    Caption = item.name,
                    Content = button,
                    LeftButtonContent = "Close",
                    RightButtonContent = "Add CheckIn", 
                };
           
            cmb.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.RightButton:
                        //Se l'utente clicca il righbutton allora invoco la funzione per aggiungere il checkin
                        addCheckIn("1",Convert.ToString(item.id), "manual", "9");
                        break;
                }
            };
            cmb.Show();
        }

        private async void addCheckIn(string devId, string poiId, string type, string userId)
        {
            string json = await WebAPI.checkin_add(devId, poiId, type, userId);
            MessageBox.Show("Aggiunto tra i CheckIn POI=" + poiId); 
            
            /* Potrei verificare se il checkin è andato a buon fine,
             * anche se i dati non sono inseriti ma recuperati in modo automatico,
             * quindi non ci dovrebbero essere problemi.
             *
             * List<CheckInStatus> checkin = new List<CheckInStatus>();
             * checkin = Utils.DeserializeJSONArray<CheckInStatus>(json);
             * if(checkin[0].Status=="Success") Ok
             * else Something is wrong.
            */
        }

        /*
         * Funzione che si attiva al tap dell'hyperlink button per i dettagli
         */
        void button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string item = ((FrameworkElement)e.OriginalSource).DataContext as string;
            NavigationService.Navigate(new Uri("/DetailPage.xaml?" + item, UriKind.Relative));
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