using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AppSDEM
{

    /**
     * Classe per i campi che voglio visualizzare nella LongListSelector
     */
    public class PoIvicini
    {
        public string idpoi { get; set; }
        public string immagine { get; set; }
        public string nome { get; set; }
        public string citta { get; set; }
        public string distanza { get; set; }
        public string posizione { get; set; }
    }

    /**
     * Controller dei risultati di ricerca dei PoI vicini
     * @author Francesco Garutti
     */
    public partial class NearbyResultsPage : PhoneApplicationPage
    {
        public NearbyResultsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // carico la lista solo la prima volta quando non è selezionato niente
            // altrimenti se entro in un dettaglio e faccio back mi torna a ricaricare 
            // di nuovo tutta la lista
            if (poivicini.SelectedItem == null)
            {
                //attivo la progress bar
                progressbar.IsIndeterminate = true;
                // ottengo la risposta in formato json dalla funzione <code>get_nearby</code>
                // e la deserializzo creando la lista <code>idpoiList</code>
                string resultjson = e.Uri.ToString();
                resultjson = NavigationContext.QueryString["result"];
                
                List<Categoria> idpoiList = new List<Categoria>();
                idpoiList = Utils.DeserializeJSONArray<Categoria>(resultjson);
                // se non ho trovato nessun PoI lo notifico all'utente con una textbox
                if (idpoiList.Count == 0)
                {
                    notfound_txt.Text = "Nessun PoI trovato all'interno di questo raggio!";
                    // stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // inizializzo la <code>listpoivicini</code> come <code>ObservableCollection</code>
                    // per creare la lista dinamica dei poi vicini trovati
                    ObservableCollection<PoIvicini> listpoivicini = new ObservableCollection<PoIvicini>();

                    // invoco la funzione di GeoLocator per ritornare la mia posizione
                    Geolocator myGeolocator = new Geolocator();
                    Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                    string currentLocation = myGeoposition.Coordinate.ToGeoCoordinate().ToString();

                    // faccio un for per ogni poi vicino trovato e ad ogni ciclo aggiungo un elemento 
                    // alla LongListSelector
                    for (int i = 0; i < idpoiList.Count; i++)
                    {
                        // invoco la funzione <code>poi_details</code> per ottenere alcuni dati
                        // significativi del poi da mostrare nella lista
                        string json_detail = await WebAPI.poi_details("1", idpoiList[i].idpoi.ToString());
                        List<PoI> detailpoiList = new List<PoI>();
                        detailpoiList = Utils.DeserializeJSONArray<PoI>(json_detail);
                        string poiposition = idpoiList[i].position;
                        // calcolo la distanza da dove sono rispetto al PoI
                        detailpoiList[0].position = poiposition;
                        // divido per 1000 poichè la funzione restituisce il risultato in metri
                        float distance = (float)(detailpoiList[0].DistanceTo(currentLocation))/1000;

                        // aggiungo il poi alla lista con i dati che mi interessano
                        listpoivicini.Add(new PoIvicini
                        {
                            idpoi = idpoiList[i].idpoi.ToString(),
                            immagine = detailpoiList[0].GetUrlThumbImage(),
                            nome = detailpoiList[0].short_description,
                            citta = detailpoiList[0].city,
                            distanza = distance.ToString(),
                            posizione = detailpoiList[0].position
                        });
                    }
                    // stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;
                    // alla fine visualizzo la <code>listpoivicini</code> tramite la
                    // LongListSelector <code>poivicini</code>
                    poivicini.ItemsSource = listpoivicini;
                }
            }
        }

        /**
         * Funzione che al Tap di un elemento della lista apre un CMB con possibilità di scelta tra
         * visualizzare più dettagli o visualizzare il PoI nella mappa
         */
        private void GetPoiDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // creo un item che ottiene i dati dell'elemento ottenuto dal tap
            PoIvicini item = ((FrameworkElement)e.OriginalSource).DataContext as PoIvicini;
            // ci entra solo se clicchi sui PoI senza immagine perchè non riesce a creare il data context
            if (item == null)
            {
                MessageBox.Show("Non ci sono ulteriori informazioni per questo PoI");
                return;
            }
            // creo un CutstomMessageBox che appare al Tap per avere più info o visualizzare il PoI sulla mappa
            CustomMessageBox cmb = new CustomMessageBox()
            {
                Caption = item.nome,
                LeftButtonContent = "More Details",
                RightButtonContent = "View on Map",
            };
            // vengono assegnati 2 metodi diversi in relazione ai 2 bottoni sul CMB
            cmb.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    // bottone sinistro -> pagina dei dettagli
                    case CustomMessageBoxResult.LeftButton:
                        NavigationService.Navigate(new Uri("/DetailPage.xaml?poi_id=" + item.idpoi, UriKind.Relative));
                        break;
                    // bottone destro -> visualizzare il checkin nella mappa
                    case CustomMessageBoxResult.RightButton:
                        NavigationService.Navigate(new Uri("/MapPage.xaml?pos=" + item.posizione, UriKind.Relative));
                        break;
                }
            };
            // mostra il CMB
            cmb.Show();
        }
    }
}