using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;
using Windows.Phone.Media.Capture;

namespace AppSDEM
{
    public partial class camera : PhoneApplicationPage
    {
        private int savedCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();
        public camera()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {  
            // Controllo se la fotocamera è disponibile nel telefono.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Inizializzo la fotocamera, se disponibile.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary))
                {
                    // Uso la fotocamera standard.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }
                else
                {
                    // Uso la fotocamera frontface.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.FrontFacing);
                }

                // Evento generato quanto il pulsante hardware è premuto a metà.
                CameraButtons.ShutterKeyHalfPressed += OnButtonHalfPress;

                //  Evento generato quanto il pulsante hardware è premuto pienamente.
                CameraButtons.ShutterKeyPressed += OnButtonFullPress;

                //  Evento generato quanto il pulsante hardware è rilasciato.
                CameraButtons.ShutterKeyReleased += OnButtonRelease;

                // Evento generato quando l'oggetto della fotocamera è stato inizializzato.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Evento generato quando quando la cattura dalla parte della fotocamera è terminata.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Evento generato quando quando la cattura dalla parte della fotocamera è terminata e l'immagine è disponibile.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                // Evento generato quando quando la cattura dalla parte della fotocamera è terminata ed è disponibile un thumbnail.
                cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);
                //Assegno la sorgente VideoBrush alla fotocamera.
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                // La fotocamera non è supportata.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Mando un messaggio d'errore.
                    txtDebug.Text = "Su questo dispositivo non è disponibile una fotocamera.";
                });

                // Disabilito la UI.
                ShutterButton.IsEnabled = false;
            }

        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Minimizzare il consumo di energia e per accelerare lo spegnimento.
                cam.Dispose();

                CameraButtons.ShutterKeyHalfPressed -= OnButtonHalfPress;
                CameraButtons.ShutterKeyPressed -= OnButtonFullPress;
                CameraButtons.ShutterKeyReleased -= OnButtonRelease;

                // Rilasciare la memoria, garantire la raccolta di quello che non serve.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
            }
        }

        // Aggiornamento dell'UI se l'inizializzazione ha buon esito.
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Mando messaggio di inizializzazione.
                    //  txtDebug.Text = "Fotocamera inizializzata.";
                });
            }
        }

        // Assicurarsi che il viewfinder è in posizione verticale nel LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                // Rotazione LandscapeRight.
                int landscapeRightRotation = 180;

                // Cambiare la rotazione LandscapeRight per il front-facing camera.
                if (cam.CameraType == CameraType.FrontFacing) landscapeRightRotation = -180;

                // Rotazione del video brush dalla camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotazione LandscapeRight.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotazione standard landscape.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        private void ShutterButton_Click(object sender, RoutedEventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Inizio la cattura dell'immagine.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        // Non posso fare una foto se l'ultima non è stata ancora catturata.
                        txtDebug.Text = ex.Message;
                    });
                }
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            //Incremento savedCounter utilizzato per genrare i nomi dei file JPEG.
            savedCounter++;
        }

        // Informa quando l'immagine è stata completata ed effettua il salvataggio.
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = savedCounter + ".jpg";

            try
            {   // Mando un messagio di completamento.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    // txtDebug.Text = "Salvataggio dell'immagine in corso.";
                });

                // Salvo l'immagine nel media library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);

                // Mando un messagio di completamento.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    // txtDebug.Text = "La foto è stata salvata nel camera roll.";

                });

                // Setto la posizione dello stream all'inizio.
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                // Salvo la foto come JPEG nella cartella locale.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Inizializzo il buffer per 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copio l'immagine nella cartella locale. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Mando un messagio di completamento.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    // txtDebug.Text = "La foto è stata salvata nella cartella locale.";

                });
            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();
            }

        }

        // Informo quando il thumbnail è stato catturato e salvo nella cartella locale.

        public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            string fileName = savedCounter + "_th.jpg";

            try
            {
                // Mando un messagio di completamento.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    // txtDebug.Text = "Salvataggio del thumbnail in corso.";
                });

                // Salvo il thumbnail come JPEG nella cartella locale.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Inizializzo il buffer per 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copio il thumbnail nella cartella locale. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Mando un messagio di completamento.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    //  txtDebug.Text = "Il thumbnail è stato salvato nella cartella locale.";

                });
            }
            finally
            {
                // Chiudo lo stream dell'immagine.
                e.ImageStream.Close();
            }
        }

        // Auto-focus con a prememendo il pulsante hardware a metà.
        private void OnButtonHalfPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                // Focus dell'immagine.
                try
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        // txtDebug.Text = "Auto Focus";
                    });

                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Non si può effettuare il focus quando sto catturando un'immagine.
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = focusError.Message;
                    });
                }
            }
        }

        // Cattura dell'immagine premendo pienamente il pulsante hardware.
        private void OnButtonFullPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                cam.CaptureImage();
            }
        }

        // Cancello il focus quando rilascio il pulsante.
        private void OnButtonRelease(object sender, EventArgs e)
        {

            if (cam != null)
            {
                cam.CancelFocus();
            }
        }


    }
}