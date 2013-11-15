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
        /** URL del server */
        const string apiUrl = @"http://jupiter.ing.unimo.it/rest/";
        /** versione delle API */
        const string apiVer = @"1/";

        /**
         * Crea un WebClient che accetta risultati di tipo <code>application/json</code>
         */
        static WebClient createClient()
        {
            WebClient wc = new WebClient();
            wc.Headers["Accept"] = "application/json";
            return wc;
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
            string url = apiUrl + apiVer + "login?device_id=" + devId + "&mail=" + mail + "&password=" + password;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    tcs.SetResult(e.Result);
                }
                else
                {
                    tcs.SetException(e.Error);
                }
            };
            wc.DownloadStringAsync(new Uri(url));

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
            string url = apiUrl + apiVer + "poi_update?device_id=" + devId + "&incremental=" +incremental;
            var tcs = new TaskCompletionSource<string>();
            wc.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    tcs.SetResult(e.Result);
                }
                else
                {
                    tcs.SetException(e.Error);
                }
            };
            wc.DownloadStringAsync(new Uri(url));

            string result = await tcs.Task;
            return result;
        }
    }
}
