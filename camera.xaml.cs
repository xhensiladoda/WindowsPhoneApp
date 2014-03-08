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
            _camtask.Show();
        }

        private void TaskCompleted(object sender, PhotoResult photo_result)
        {
            byte[] _local_image;
            if (photo_result.ChosenPhoto != null)
            {
                _local_image = new byte[(int)photo_result.ChosenPhoto.Length];
                photo_result.ChosenPhoto.Read(_local_image, 0, _local_image.Length);
                photo_result.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
                var bitmapImage = PictureDecoder.DecodeJpeg(photo_result.ChosenPhoto);
                captured_image.Source = bitmapImage;
            }
        }
        
    }
}