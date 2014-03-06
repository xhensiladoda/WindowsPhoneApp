using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;

namespace AppSDEM
{
    /**
     * Controller dei risultati di ricerca dei PoI
     * @author Francesco Garutti
     */

    /**
     * Classe per i campi che voglio visualizzare nella LongListSelector
     */ 
    public class PoItrovati
    {
        public string idpoi { get; set; }
        public string immagine { get; set; }
        public string nome { get; set; }
        public string citta { get; set; }
        public string posizione { get; set; }
        public string distanza { get; set; }
    }

    public partial class PoISearchResultsPage : PhoneApplicationPage
    {
        public PoISearchResultsPage()
        {
            InitializeComponent();
        }

        /**
         * Funzione che parte con la ricerca dei PoI
         * restituisce la lista dei PoI trovati
         */ 
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // carico la lista solo la prima volta quando non è selezionato niente
            // altrimenti se entro in un dettaglio e faccio back mi torna a ricaricare 
            // di nuovo tutta la lista
            if (poitrovati.SelectedItem == null)
            {
                //attivo la progress bar
                progressbar.IsIndeterminate = true;
                // ottengo la risposta in formato json dal form di ricerca e la deserializzo
                // creando la lista <code>idpoiList</code>
                string resultjson = e.Uri.ToString();
                resultjson = resultjson.Substring(resultjson.IndexOf("?") + 1);
                List<PoI> idpoiList = new List<PoI>();
                idpoiList = Utils.DeserializeJSONArray<PoI>(resultjson);
                // se non ho trovato nessun PoI lo notifico all'utente con una textbox
                if (idpoiList.Count == 0)
                {
                    notfound_txt.Text = "Nessun PoI trovato tramite questa ricerca!";
                    // stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;                 
                }
                else
                {
                    // inizializzo la <code>listpoitrovati</code> come <code>ObservableCollection</code>
                    // per creare la lista dinamica dei poi trovati
                    ObservableCollection<PoItrovati> listpoitrovati = new ObservableCollection<PoItrovati>();

                    // invoco la funzione di GeoLocator per ritornare la mia posizione
                    Geolocator myGeolocator = new Geolocator();
                    Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                    string currentLocation = myGeoposition.Coordinate.ToGeoCoordinate().ToString();

                    // faccio un for per ogni poi trovato e a ogni ciclo aggiungo un elemento 
                    // alla LongListSelector
                    for (int i = 0; i < idpoiList.Count; i++)
                    {
                        // invoco la funzione <code>poi_details</code> per ottenere alcuni dati
                        // significativi del poi da mostrare nella lista
                        string json_detail = await WebAPI.poi_details("1", idpoiList[i].idpoi.ToString());
                        List<PoI> detailpoiList = new List<PoI>();
                        detailpoiList = Utils.DeserializeJSONArray<PoI>(json_detail);
                        // divido per 1000 poichè la funzione restituisce il risultato in metri
                        float distance = (float)(detailpoiList[0].DistanceTo(currentLocation)) / 1000;
                        
                        // aggiungo il poi alla lista con i dati che mi interessano
                        listpoitrovati.Add(new PoItrovati
                        {
                            idpoi = idpoiList[i].idpoi.ToString(),
                            immagine = detailpoiList[0].GetUrlThumbImage(),
                            nome = detailpoiList[0].short_description,
                            citta = detailpoiList[0].city,
                            posizione = detailpoiList[0].position,
                            distanza = distance.ToString()
                        });
                    }
                    // stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;
                    // alla fine visualizzo la <code>listpoitrovati</code> tramite la
                    // LongListSelector <code>poitrovati</code> in ordine di distanza               
                    poitrovati.ItemsSource = listpoitrovati.OrderBy(PoItrovati=>PoItrovati.distanza).ToList();
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
            PoItrovati item = ((FrameworkElement)e.OriginalSource).DataContext as PoItrovati;
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