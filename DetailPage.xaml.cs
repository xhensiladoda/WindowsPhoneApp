using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace AppSDEM
{
    public partial class DetailPage : PhoneApplicationPage
    {
        public DetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string poi_id = NavigationContext.QueryString["poi_id"];
            Left.Text = poi_id;
            string json = await WebAPI.poi_details("2", poi_id);
            List<PoI> poiList = new List<PoI>();
            poiList = Utils.deserializeJSONArray<PoI>(json);
            PoI myPoI = poiList[0];
            this.titolo.Text = myPoI.city;
            // TODO: inserire maggiori informazioni sul poi nella pagina
        }
    }
}
