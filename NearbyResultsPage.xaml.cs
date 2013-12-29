using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
     * Classe per i campi che voglio visualizzare nella LongListSelector
     */
    public class PoIvicini
    {
        public string idpoi { get; set; }
        public string immagine { get; set; }
        public string nome { get; set; }
        public string citta { get; set; }
        public string distanza { get; set; }
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
                resultjson = resultjson.Substring(resultjson.IndexOf("result=") + 7);
                string myposition = NavigationContext.QueryString["pos"];

                List<Categoria> idpoiList = new List<Categoria>();
                idpoiList = Utils.deserializeJSONArray<Categoria>(resultjson);
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

                    // faccio un for per ogni poi vicino trovato e ad ogni ciclo aggiungo un elemento 
                    // alla LongListSelector
                    for (int i = 0; i < idpoiList.Count; i++)
                    {
                        // invoco la funzione <code>poi_details</code> per ottenere alcuni dati
                        // significativi del poi da mostrare nella lista
                        string json_detail = await WebAPI.poi_details("1", idpoiList[i].idpoi.ToString());
                        List<PoI> detailpoiList = new List<PoI>();
                        detailpoiList = Utils.deserializeJSONArray<PoI>(json_detail);
                        string position = idpoiList[i].position;

                        // calcolo la distanza da dove sono rispetto al PoI tramite
                        // la funzione <code>PoIDistance</code>
                        float distance = PoIDistance(myposition, position);

                        // aggiungo il poi alla lista con i dati che mi interessano
                        listpoivicini.Add(new PoIvicini
                        {
                            idpoi = idpoiList[i].idpoi.ToString(),
                            immagine = detailpoiList[0].GetUrlThumbImage(),
                            nome = detailpoiList[0].short_description,
                            citta = detailpoiList[0].city,
                            distanza = distance.ToString()
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

        private float PoIDistance(string myposition, string position)
        {
            // raggio medio della Terra
            float earth_radius = 6371f;

            // estraggo latitudine e longitudine di dove mi trovo e le trasformo in radianti
            string[] mypos = myposition.Split(new char[] { '-' });
            float mylat = float.Parse(mypos[0]);
            float mylon = float.Parse(mypos[1]);
            float mylatRad = DegreeToRadians(mylat);
            float mylonRad = DegreeToRadians(mylon);

            // estraggo latitudine e longitudine del PoI e le trasformo in radianti
            string[] pos = position.Split(new char[] { ',' });
            float lat = float.Parse(pos[0], CultureInfo.InvariantCulture);
            float lon = float.Parse(pos[1], CultureInfo.InvariantCulture);
            float latRad = DegreeToRadians(lat);
            float lonRad = DegreeToRadians(lon);

            // tramite la trigonometria calcolo la distanza spaziale tra dove sono e il PoI
            float phi = Math.Abs(mylonRad - lonRad);
            float point = (float)Math.Acos((Math.Sin(mylatRad) * Math.Sin(latRad)) + (Math.Cos(mylatRad) * Math.Cos(latRad) * Math.Cos(phi)));
            float distance = point * earth_radius;

            // ritorno la distanza
            return distance;
        }

        /**
         * Funzione che trasforma un angolo da gradi in radianti
         */ 
        private float DegreeToRadians(float angle)
        {
            return (float)Math.PI * angle / 180f;
        }

        /**
         * Funzione che al Tap di un elemento della lista ti indirizza alla pagina
         * <code>DetailPage</code> per visualizzare tutti i dettagli del PoI
         */ 
        private void GetPoiDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // creo un item che ottiene i dati dell'elemento ottenuto dal tap
            PoIvicini item = ((FrameworkElement)e.OriginalSource).DataContext as PoIvicini;
            NavigationService.Navigate(new Uri("/DetailPage.xaml?poi_id=" + item.idpoi, UriKind.Relative));
        }
    }
}