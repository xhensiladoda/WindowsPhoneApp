using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AppSDEM
{
    /**
     * Controller dei risultati di ricerca del CheckIn
     * @author Xhensila Doda
     */

    public class CheckInDisponibili
    {
        public string descrizione { get; set; }
        public string immagine { get; set; }
        public string nome { get; set; }
    }

    public partial class CheckInResult : PhoneApplicationPage
    {
        public CheckInResult()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (listacheckin.SelectedItem == null) {
                progressbar.IsIndeterminate = true;
                string json = await WebAPI.get_user_checkin("1","9","True");
                List<CheckIn> checkin = new List<CheckIn>();
                checkin = Utils.DeserializeJSONArray<CheckIn>(json);
                if (checkin.Count == 0)
                {
                    notfound_txt.Text = "Nessun CheckIn disponibile per questo utente!";
                    // stoppo e nascondo la progress bar
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;

                }
                else
                {
                    ObservableCollection<CheckInDisponibili> checkin_trovati= new ObservableCollection<CheckInDisponibili>();
                    for (int i = 0; i < checkin.Count; i++)
                    {
                        checkin_trovati.Add(new CheckInDisponibili
                        { descrizione=checkin[i].description,
                          nome=checkin[i].name,
                          immagine=checkin[i].GetImage(),
                        });
                    
                    }
                    progressbar.IsIndeterminate = false;
                    progressbar.Visibility = Visibility.Collapsed;
                    listacheckin.ItemsSource = checkin_trovati;
                }

            
            }
        
        
        }
    }
}