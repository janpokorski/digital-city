using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Plugin.Geolocator;
using Plugin.LocalNotifications;

namespace DigitalCity
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            DependencyService.Get<ILocationManager>().GetPermissions();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingPage());
        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            DependencyService.Get<ILocationManager>().StartLocationUpdates();
            DependencyService.Get<ILocationManager>().LocationUpdated += LocationUpdateHandler;


            var address = inputEntry.Text;

            switch(Device.RuntimePlatform)
            {
                case Device.iOS: Device.OpenUri(new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                    break;
                case Device.Android: Device.OpenUri(new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                    break;
            }


        }

        void LocationUpdateHandler(object sender, LocationEventArgs e)
        {
            if(e == null)
            {
                CrossLocalNotifications.Current.Show("Warning", "Location service is not available");
                return;
            }
            CrossLocalNotifications.Current.Show("Test", string.Format("{0}, {1}", e.Latitude, e.Longitude));

        }
	}
}
