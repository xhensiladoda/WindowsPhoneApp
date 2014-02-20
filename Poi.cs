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
     * Classe che rappresenta un Point of Interest (PoI) e che ne definisce
     * la sua serializzazione in formato JSON.
     * @author Setti Davide
     */
    [DataContract]
    class PoI
    {
        /** URL da cui reperire le immagini */
        const string SERVER_IMAGE_URL = @"http://jupiter.ing.unimo.it/media/";
        /** Identificatore del PoI */
        [DataMember]
        public int idpoi { get; set; }
        /** Identificatore della  categoria */
        [DataMember]
        public int idcategory { get; set; }
        /** Identificativo dell'admin */
        [DataMember]
        public string admin_id { get; set; }
        /** Data di creazione del PoI */
        [DataMember]
        public DateTime creation_date { get; set; }
        /** Data di ultima modifica del PoI */
        [DataMember]
        public DateTime modified_date { get; set; }
        /** url relativo dell'immagine del PoI */
        [DataMember]
        public string image { get; set; }
        /** url relativo dell'immagine thumbnail del PoI */
        [DataMember]
        public string thumbnail { get; set; }
        /** url assoluto del sito web del PoI */
        [DataMember]
        public Uri url { get; set; }
        /** Nome del PoI*/
        [DataMember]
        public string name { get; set; }
        /** Descrizione estesa del PoI */
        [DataMember]
        public string long_description { get; set; }
        /** Descrizione sintetica del PoI */
        [DataMember]
        public string short_description { get; set; }
        /** Nazione di localizzazione del PoI */
        [DataMember]
        public string country { get; set; }
        /** Provincia di localizzazione del PoI */
        [DataMember]
        public string province { get; set; }
        /** Città di localizzazione del PoI */
        [DataMember]
        public string city { get; set; }
        /** Zipcode (codice postale) del PoI */
        [DataMember]
        public string zipcode { get; set; }
        /** Indirizzo di localizzazione del PoI */
        [DataMember]
        public string address { get; set; }
        /** Contatto email del PoI */
        [DataMember]
        public string email { get; set; }
         /** Contatto telefonico del PoI */
        [DataMember]
        public string telephone { get; set; }
        /** Stringa con la posizione del PoI nel formato lat,lon */
        [DataMember] 
        public string position { get; set; }
        /** Costruttore vuoto di default */
        public PoI () {}

        /**
         * Crea un oggetto GeoCoordinate con le coordinate del PoI.
         * @return: GeoCoordinate del PoI.
         */
        public GeoCoordinate GetCoordinate()
        {
            return Utils.String2GeoCoordinate(position);
        }

        /**
         * Calcola la distanza, in metri, tra il PoI ed un punto la cui posizione è data
         * nel formato "lat,lon".
         * @return: distanza in metri del PoI dal punto indicato.
         */
        public double DistanceTo(string otherPos)
        {
            // codice abbastanza brutale con poche variabili temporanee
            return GetCoordinate().GetDistanceTo(Utils.String2GeoCoordinate(otherPos));
        }

        /**
         * Carica l'immagine thumbnail del PoI dal server.
         * @return: thumbnail del PoI
         */
        public Image GetThumbImage()
        {
            return GetImage(thumbnail);
        }

        /**
         * Carica l'immagine normale del PoI dal server.
         * @return: immagine del PoI
         */
        public Image GetNormImage()
        {
            return GetImage(image);
        }

        /**
         * Carica l'immagine specificata dal server.
         * @param img: indirizzo relativo dell'immagine nella directory server
         * @return: l'immagine selezionata
         */
        private Image GetImage(string img)
        {
            string url = SERVER_IMAGE_URL + img;
            Image image = new Image();
            image.Source = new System.Windows.Media.Imaging.BitmapImage(
            new Uri(url, UriKind.RelativeOrAbsolute));
            return image;
        }

        /**
         * Ottiene l'indirizzo url dell'immagine specificata dal server
         * @param img: indirizzo relativo dell'immagine nella directory server
         * @return: url dell'immagine selezionata
         */ 
        private string GetUrlImage(string img)
        {
            string url = SERVER_IMAGE_URL + img;
            return url;
        }

        /**
         * Ottiene l'url dell'immagine thumbnail del PoI dal server
         * @return: url del thumbnail del PoI
         */ 
        public string GetUrlThumbImage()
        {
            return GetUrlImage(thumbnail);
        }

        /**
       * Ottiene l'url dell'immagine normale del PoI dal server
       * @return: url dell'immagine normale del PoI
       */ 
        public string GetUrlNormImage()
        {
            return GetUrlImage(image);
        }
    }
}
