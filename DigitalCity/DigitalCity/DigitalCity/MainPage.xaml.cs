using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Plugin.Geolocator;
using Plugin.LocalNotifications;
using System.Net.Http;
using Newtonsoft.Json;
using DigitalCity.Model;

namespace DigitalCity
{
	public partial class MainPage : ContentPage
	{
        //set if debugging or not
        private bool DEBUG_MODE = true;

        HttpClient client;

		public MainPage()
		{
			InitializeComponent();

            ToolbarItem item = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    item = new ToolbarItem
                    {
                        Text = "Settings",
                        Order = ToolbarItemOrder.Primary
                    };
                    item.Clicked += Handle_Clicked;
                    break;
                case Device.Android:
                    item = new ToolbarItem
                    {
                        Text = "Settings",
                        Order = ToolbarItemOrder.Secondary
                    };
                    item.Clicked += Handle_Clicked;
                    break;
            }

            this.ToolbarItems.Add(item);
            DependencyService.Get<INotification>().GetPermissions();
            DependencyService.Get<ILocationManager>().GetPermissions();
            CreateHttpClient(50000);

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

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Device.OpenUri(new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                    break;
                case Device.Android:
                    Device.OpenUri(new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                    break;
            }

        }

        void LocationUpdateHandler(object sender, LocationEventArgs e)
        {
            if(e == null)
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the location service", 3));
                return;
            }

            if(Device.RuntimePlatform == Device.Android){
                DependencyService.Get<ILocationManager>().StopLocationUpdates();
                GetSensorData(e);
                DependencyService.Get<ILocationManager>().StartLocationUpdates();
            }
            else if (Device.RuntimePlatform == Device.iOS){
                GetSensorData(e);
            }


        }

        void CreateHttpClient(int bufferSize){
            client = new HttpClient();
            client.MaxResponseContentBufferSize = bufferSize;
        }

        void GetSensorData(LocationEventArgs args){
            GetRoadSensorData(args);
            GetLaneSensorData(args);
            GetLightSensorData(args);
            GetEnvSensorData(args);
        }


        async void GetRoadSensorData(LocationEventArgs args)
        {
            if (!EnabledNotifications.weather)
                return;
            
            string uri = "http://mobiheaven.de/digital-city/index.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "200";

            uri += "&type=roadSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                if (collection.roadSensors.Length > 0)
                    ProcessRoadSensor(collection.roadSensors[random.Next(collection.roadSensors.Length)], args);
            }
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        async void GetLaneSensorData(LocationEventArgs args)
        {
            if (!EnabledNotifications.jams)
                return;
            
            string uri = "http://mobiheaven.de/digital-city/index.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";

            uri += "500";

            uri += "&type=laneSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                if (collection.laneSensors.Length > 0)
                    ProcessTraficLaneSensor(collection.laneSensors[random.Next(collection.laneSensors.Length)], args);
            }
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        async void GetLightSensorData(LocationEventArgs args)
        {
            if (!EnabledNotifications.lights)
                return;

            string uri = "http://mobiheaven.de/digital-city/index.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "300";

            uri += "&type=lightSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                if (collection.lightSensors.Length > 0)
                    ProcessTrafficLightSensor(collection.lightSensors[random.Next(collection.lightSensors.Length)], args);
            }
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        async void GetEnvSensorData(LocationEventArgs args)
        {
            if(!EnabledNotifications.pollution)
                return;
            
            string uri = "http://mobiheaven.de/digital-city/index.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "1000";

            uri += "&type=envSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                if (collection.envSensors.Length > 0)
                    ProcessEnvirontmentalSensor(collection.envSensors[random.Next(collection.envSensors.Length)], args);
            }
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        void ProcessEnvirontmentalSensor(Model.EnviromentalSensor sensor, LocationEventArgs args){
             if (sensor.activeOxygen > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Oxygen has exceeded the threshold of 200 ppm", 1));
             if (sensor.carbonMonoxidePPM > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon monoxide has exceeded the threshold of 200 ppm", 3));
             if (sensor.carbonDioxidePPM > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon dioxide has exceeded the threshold of 200 ppm", 2));
        }

        void ProcessRoadSensor(Model.RoadSensor sensor, LocationEventArgs args){
                string content = string.Format("The outdoor temperature is {0} degres. ", sensor.currentEnviromentTemperature);

                string imagePath = null;

                var notification = new Model.Notification(Model.Notification.Type.Weather, "Weather", content, imagePath, 0);

                if(sensor.currentEnviromentTemperature <= 0){
                    notification.imagePath = "snow4";
                }
                else if (sensor.humidityLevel >= 40 && sensor.humidityLevel <= 60){
                    notification.imagePath = "cloudy2";
                    
                }
                else if(sensor.humidityLevel > 60){
                    notification.imagePath = "shower3";
                }
                else{
                    notification.imagePath = "sunny";
                }
                 
                 DependencyService.Get<INotification>().SendCollapsedNotification(notification);
        }

        void ProcessTrafficLightSensor(Model.TrafficLightSensor sensor, LocationEventArgs args){
                if(sensor.trafficLightState.Equals("red")){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is red.", 1));
                }
                    else if(sensor.trafficLightState.Equals("green")){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is green.", 1));
                }
        }

        void ProcessTraficLaneSensor(Model.TrafficLaneSensor sensor, LocationEventArgs args){
                if(sensor.crossingsCounter5Minutes < 20 && sensor.averageSpeed < 40){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficJam, "Traffic jam", "There is a traffic jam ahead.", 2));
                }
        }
	}
}
