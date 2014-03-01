using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AppSDEM
{
    /**
    * Classe che rappresenta l'utente e che ne definisce
    * la sua serializzazione in formato JSON.
    * @author Doda Xhensila e Francesco Garutti
    */
    [DataContract]
    class User
    {
        /** Identificatore della primary key */
        [DataMember]
        public int pk { get; set; }
        /** Model */
        [DataMember]
        public string model { get; set; }
        /** Fields*/
        [DataMember]
        public fields fields { get; set; }

        public User() { }
    }

    [DataContract]
    class fields
    {
        [DataMember]
        public string first_name { get; set; }

        public fields() { }
    }

}
