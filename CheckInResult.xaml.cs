﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace AppSDEM
{
    /**
     * Controller dei risultati di ricerca del CheckIn
     * @author Xhensila Doda
     */

    //Struct che serve per la rappresentazione dei checkin
    public class CheckInDisponibili
    {
        // public string descrizione { get; set; }
        public string immagine { get; set; }
        public string nome { get; set; }
        public string data { get; set; }
        public string idpoi { get; set; }
    }

    public partial class CheckInResult : PhoneApplicationPage
    {
        public CheckInResult()
        {
            InitializeComponent();
        }

         /**  
         * Funzione che visualizza la lista dei CheckIn effettuati dall'utente.
         */
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Controllo se la lista è vuota all'inizio.
            if (listacheckin.SelectedItem == null) 
            {
                progressbar.IsIndeterminate = true;

                int userPK = (int)IsolatedStorageSettings.ApplicationSettings["userPK"];

                // Chiamo l'API per visualizzare i checkin dell'utente registrato
                string json = await WebAPI.get_user_checkin("1", userPK.ToString(), "True");
                List<CheckIn> checkin = new List<CheckIn>();
                checkin = Utils.DeserializeJSONArray<CheckIn>(json);

                //Se non ci sono checkin per quest'utente visualizzo un messaggio
                if (checkin.Count == 0)
                {
                    notfound_txt.Text = "Nessun CheckIn disponibile per questo utente!";
                    // Stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;

                }
                else //altrimenti
                {   
                    // salvo i dettagli che andrò a visualizzare.
                    ObservableCollection<CheckInDisponibili> checkin_trovati= new ObservableCollection<CheckInDisponibili>();
                    
                    //Salvo i dati nella struct per la visualizzazione.
                    for (int i = 0; i < checkin.Count; i++)
                    {
                        checkin_trovati.Add(new CheckInDisponibili
                        { 
                          // descrizione=checkin[i].description,
                          nome = checkin[i].name,
                          immagine = checkin[i].GetImage(),
                          data = checkin[i].date.ToString(),
                          idpoi = checkin[i].idpoi.ToString()
                        });
                    
                    }
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;
                    listacheckin.ItemsSource = checkin_trovati;
                }   
            }
        }

        /**  
         * Funzione che visualizza la lista dei dettagli del checkin oppure la sua posizione nella mappa geografica.
         */
        private async void listacheckin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Creo un item che ottiene i dati dell'elemento ottenuto dal tap
            CheckInDisponibili item = ((FrameworkElement)e.OriginalSource).DataContext as CheckInDisponibili;
            
            // Faccio un poi_details per estrarre la posizione del checkin
            string json_detail = await WebAPI.poi_details("1", item.idpoi);
            List<PoI> detail_checkin_List = new List<PoI>();
            detail_checkin_List = Utils.DeserializeJSONArray<PoI>(json_detail);
            string checkin_position = detail_checkin_List[0].position;

            // Creo un CutstomMessageBox che appare al Tap per avere più info o visualizzare il checkin sulla mappa
            CustomMessageBox cmb = new CustomMessageBox()
            {
                Caption = item.nome,        
                LeftButtonContent = "More Details",
                RightButtonContent = "View on Map",
            };
            // Vengono assegnati 2 metodi diversi in relazione ai 2 bottoni sul CMB
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
                            NavigationService.Navigate(new Uri("/MapPage.xaml?pos=" + checkin_position, UriKind.Relative));
                            break;
                    }
                };
            // mostra il CMB
            cmb.Show();
        }
    }
}