﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    }

    
}
