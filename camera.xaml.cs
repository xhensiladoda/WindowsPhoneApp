using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;

namespace AppSDEM
{   
    /**
    * Controller responsabile per l'accesso alla fotocamera.
    * @author Doda Xhensila
    */
    public partial class camera : PhoneApplicationPage
    {
        //crea un oggetto CameraCaptureTask per l'accesso al task della fotocamera del dispositivo
        private CameraCaptureTask _camtask;
       
        public camera()
        {
            InitializeComponent();
            InitializeComponent();
            _camtask = new CameraCaptureTask();
            _camtask.Completed += TaskCompleted;
        }

        private void camera_button_click(object sender, RoutedEventArgs e)
        {   
            //avvio il task della fotocamera al click dell'utente
            _camtask.Show();
        }

        /*Questa funzione permette di salvare l'immagine nel dispositivo. */         
        private void TaskCompleted(object sender, PhotoResult photo_result)
        {
            byte[] _local_image;
            //se lo stream dei dati riguardo alla foto scattata non è nullo si prosegue con il salvataggio.
            if (photo_result.ChosenPhoto != null)
            {   
                //creo lo stream dove salvare i dati
                _local_image = new byte[(int)photo_result.ChosenPhoto.Length];
                //lo stream dei dati sarà salvato in _local_image
                photo_result.ChosenPhoto.Read(_local_image, 0, _local_image.Length);
                //mi posiziono all'inizio dello stream dei dati per il salvataggio
                photo_result.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
                //effettuo la conversione in un formato compatibile e leggibile
                var bitmapImage = PictureDecoder.DecodeJpeg(photo_result.ChosenPhoto);
                //visualizzo nel panello una preview dell'immagine appena salvata
                captured_image.Source = bitmapImage;
            }
        }
        
    }
}
