using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AppSDEM
{
    [DataContract]
    class Categoria
    {
        /** URL da cui reperire le immagini */
        const string SERVER_IMAGE_URL = @"http://jupiter.ing.unimo.it/media/";
        /** Data di ultima modifica della categoria */
        [DataMember]
        public DateTime modified_date { get; set; }
        /** Identificatore della  categoria */
        [DataMember]
        public int idcategory { get; set; }
        /** Identificatore del PoI */
        [DataMember]
        public int idpoi { get; set; }
        /** Nome del PoI*/
        [DataMember]
        public string name { get; set; }
        /** Descrizione della categoria */
        [DataMember]
        public string description { get; set; }
        /** url relativo dell'immagine della categoria */
        [DataMember]
        public string image { get; set; }
        /** Stringa con la posizione del PoI nel formato lat,lon */
        [DataMember]
        public string position { get; set; }

        public Categoria() {}

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
        * Carica l'immagine normale del PoI dal server.
        * @return: immagine del PoI
        */
        public Image GetNormImage()
        {
            return GetImage(image);
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
        * Ottiene l'url dell'immagine normale del PoI dal server
        * @return: url dell'immagine normale del PoI
        */
        public string GetUrlNormImage()
        {
            return GetUrlImage(image);
        }

    }
}
