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
    * Classe che rappresenta il risultato dell'operazione dopo aver effettuato il
    * CheckIn e ne definisce la sua serializzazione in formato JSON.
    * @author Xhensila Doda
    */
    class CheckInStatus
    {
        /** Stato dell'operazione di Checkin. Vale Success se essa è andata a buon fine*/
        [DataMember]
        public string Status { get; set; }

        public CheckInStatus() { }
    }
}
