using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Device.Location;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media;

namespace AppSDEM
{  
    /**
    * Classe che rappresenta i CheckIn di un utente e
    * ne definisce la sua serializzazione in formato JSON.
    * @author Xhensila Doda
    */
    [DataContract]
    class CheckIn
    {
        /** URL da cui reperire le immagini */
        const string SERVER_IMAGE_URL = @"http://jupiter.ing.unimo.it/media/";

        /** Identificatore del checkin */
        [DataMember]
        public int idcheckin { get; set; }

        /** Identificatore del POI */
        [DataMember]
        public int idpoi { get; set; }

        /** Descrizione */
        [DataMember]
        public string description { get; set; }

        /** Nome */
        [DataMember]
        public string name { get; set; }

        /** Immagine */
        [DataMember]
        public string image { get; set; }

        /** Data */
        [DataMember]
        public DateTime date { get; set; }

        public CheckIn() { }

        /** Funzione che permette di recuperare l'url dell'immagine nel server.
         * @param img: immagine che è recuperata dalla deserializzazione del json.
         */
        private string GetUrlImage(string img)
        {
            string url = SERVER_IMAGE_URL + img;
            return url;
        }

        /** 
         * Funzione che restituisce l'immagine.
         */
        public string GetImage()
        {
            return GetUrlImage(image);
        }
    }
}
