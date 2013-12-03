using System;
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
            string url = SERVER + LOGIN_API + "?device_id=" + devId + "&mail=" + mail + "&password=" + password;
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

		// TODO: implementare le altre web API
    }
}
