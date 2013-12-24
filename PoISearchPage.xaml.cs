﻿using System;
using System.Collections.Generic;
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
     * Controller del form di ricerca dei PoI
     * @author Garutti Francesco
     */

    public partial class PoISearchPage : PhoneApplicationPage
    {
        public PoISearchPage()
        {
            InitializeComponent();
        }

        /**
         * Funzione che al click del pulsante <code>Cerca!</code> carica i dati del form
         * e tramite l'API <code>poi_search</code> restituisce il risultato
         * TODO: completare descrizione in base a quel che si farà col risultato
         */
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {

            // estraggo i campi dalle textbox del form di ricerca
            String latitudine = txt_latitudine.Text;
            String longitudine = txt_longitudine.Text;
            String raggio = txt_raggio.Text;
            String nome = txt_nome.Text;
            String categoria = txt_categoria.Text;
            String descrizione = txt_descrizione.Text;
            String indirizzo = txt_indirizzo.Text;
            String citta = txt_citta.Text;
            String provincia = txt_provincia.Text;
            String cap = txt_cap.Text;
            String nazione = txt_nazione.Text;

            float latitude, longitude, radius;

            // se l'utente non mi fornisce criteri di ricerca in base a latitudine, longitudine
            // e raggio, metto dei valori di default in modo da considerare tutto senza limiti
            if ((latitudine.Equals("")) && (longitudine.Equals("")) && (raggio.Equals("")))
            {
                latitude = 0;
                longitude = 0;
                radius = 10000;
            }
            else
            {
                // se l'utente non setta tutti e 3 i parametri gli notifico l'errore tramite
                // una MessageBox, dato che devono essere presenti tutti e 3 per la ricerca ed
                // esco alla funzione in modo che l'utente debba di nuovo cliccare sul bottone
                if ((latitudine.Equals("")) || (longitudine.Equals("")) || (raggio.Equals("")))
                {
                    MessageBox.Show("Se vuoi ricercare per latitudine-longitudine-raggio " +
                        "tutti e 3 i parametri devono essere presenti nella ricerca");
                    return;
                }

                latitude = float.Parse(latitudine, CultureInfo.InvariantCulture);
                longitude = float.Parse(longitudine, CultureInfo.InvariantCulture);
                radius = float.Parse(raggio, CultureInfo.InvariantCulture);
            }

            // chiamo l'API poi_search per ottenere il risultato della ricerca
            string result = await WebAPI.poi_search("1", latitude, longitude, radius, 
                nome, categoria, descrizione, indirizzo, citta, provincia, cap, nazione);

            
            NavigationService.Navigate(new Uri("/PoISearchResultsPage.xaml?" + result, UriKind.Relative));


            //MessageBox.Show(result);

        }
    }
}