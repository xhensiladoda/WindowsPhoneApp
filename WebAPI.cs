﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace AppSDEM
{
    /**
     * Gestore delle chiamate alle web API
     * @author Setti Davide
     */
    static class WebAPI
    {
        /** Indirizzo web del server */
        const string SERVER = @"http://jupiter.ing.unimo.it";
        /** URI rest API */
        const string API = @"/rest/";
        /** versione delle API */
        const string API_VER = @"1/";
        /** URI dell'API di login */
        const string LOGIN_API = @"/24media/rest/login/";

        /**
         * Utility che crea un WebClient che accetta risultati di tipo <code>application/json</code>
         */
        private static WebClient createClient()
        {
            WebClient wc = new WebClient();
            wc.Headers["Accept"] = "application/json";
            return wc;
        }

        /**
         * Utility per ottenere in maniera asincrona i risultati delle chiamate alle WebAPI
         * tramite un oggetto <code>TaskCompletionSource</code>
         */
        private static void asyncResponseReceive(object sender, DownloadStringCompletedEventArgs e)
        {
            var tcs = e.UserState as TaskCompletionSource<string>;
            if (e.Error == null)
            {
                tcs.SetResult(e.Result);
            }
            else
            {
                tcs.SetException(e.Error);
            }
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione di login
         * @param devId: device id
         * @param mail: email di login
         * @param password: password di login
         */
        public static async Task<string> login(string devId, string mail, string password)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + LOGIN_API + "?device_id=" + devId + "&user=" + mail + "&password=" + password;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione poi_update
         * @param devId: device id
         * @param incremental: <code>True</code> per le informazioni solo sui POI recenti, <code>False</code>
         * per le informazioni su tutti i poi
         */
        public static async Task<string> poi_update(string devId, bool incremental)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "poi_update?device_id=" + devId + "&incremental=" + incremental;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione poi_details
         * @param devId: device id
         * @param poi_id: id del poi di cui si vuole restituire il dettaglio
         * @author Garutti Francesco
         */
        public static async Task<string> poi_details(string devId, string poiId)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "poi_details?device_id=" + devId + "&poi_id=" + poiId;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione get_nearby
         * @param devId: device id
         * @param user_id: user id
         * @param latitude: latitudine di dove ti trovi
         * @param longitude: longitudine di dove ti trovi
         * @param radius: raggio di ricerca dei poi da dove ti trovi (massimo 10 poi)
         * @author Garutti Francesco
         */
        public static async Task<string> get_nearby(string devId, string userId, float latitude, float longitude, float radius)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "get_nearby?device_id=" + devId + "&user_id=" + userId
                + "&latitude=" + latitude + "&longitude=" + longitude + "&radius=" + radius;
            url = url.Replace(',', '.');
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }


        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione categories_update
         * @param devId: device id
         * @param incremental: <code>True</code> per le informazioni solo sulle categorie aggiunte recentemente,
         * <code>False</code> per le informazioni su tutte le categorie
         * @author Garutti Francesco
         */
        public static async Task<string> categories_update(string devId, bool incremental)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "categories_update?device_id=" + devId + "&incremental=" + incremental;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione poi_ids
         * @param devId: device id
         * @author Garutti Francesco
         */
        public static async Task<string> poi_ids(string devId)
        {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "poi_ids?device_id=" + devId;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }


        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione poi_search
         * @param devId: device id
         * @param latitude: latitudine di dove ti trovi
         * @param longitude: longitudine di dove ti trovi
         * @param radius: raggio di ricerca dei poi da dove ti trovi
         * @param nome: nome del poi
         * @param categoria: categoria di appartenenza del poi
         * @param descrizione: descrizione del poi
         * @param indirizzo: indirizzo del poi
         * @param città: città del poi
         * @param provincia: provincia del poi
         * @param CAP: CAP del poi
         * @param nazione: nazione del poi
         * @author Garutti Francesco
        */
        public static async Task<string> poi_search(string devId, float latitude, float longitude, 
            float radius, string name, string category, string description, string address,
            string city, string province, string zipcode, string country)
        {

            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "poi_search?device_id=" + devId
                + "&latitude=" + latitude + "&longitude=" + longitude + "&radius=" + radius + "&name=" + name
                + "&idcategory=" + category + "&long_description=" + description + "&address=" + address
                + "&city=" + city + "&province=" + province + "&zipcode=" + zipcode + "&country=" + country;

            url = url.Replace(',', '.');
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione get_user_checkin
         * @param devId: device id
         * @param userId: user id
         * @param all: 
         * @author Xhensila Doda
        */

        public static async Task<string> get_user_checkin(string devId, string userId, string all) {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "get_user_checkin?device_id=" + devId + "&user_id=" + userId + "&all=" + all;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }

        /**
         * Chiama in maniera asincrona il server e restituisce il risultato della funzione checkin_add
         * @param devId: device id
         * @param poiId: POI id
         * @param type: tipo di checkin
         * @param userId: user id
         * @author Xhensila Doda
        */
        public static async Task<string> checkin_add(string devId, string poiId, string type, string userId) {
            WebClient wc = WebAPI.createClient();
            // url da chiamare
            string url = SERVER + API + API_VER + "checkin_add?device_id=" + devId + "&poi_id=" + poiId +
                "&type=" + type + "&user_id="+ userId;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(asyncResponseReceive);
            wc.DownloadStringAsync(new Uri(url), tcs);
            string result = await tcs.Task;
            return result;
        }


		// TODO: implementare le altre web API
    }
}
