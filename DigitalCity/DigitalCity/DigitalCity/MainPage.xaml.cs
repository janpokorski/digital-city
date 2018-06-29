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

namespace DigitalCity
{
	public partial class MainPage : ContentPage
	{
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
                DependencyService.Get<INotification>().SendDefaultNotification(999, "Warning", "There is an issue with the location service");
                return;
            }

            if(Device.RuntimePlatform == Device.Android){
                DependencyService.Get<ILocationManager>().StopLocationUpdates();
                ProcessSensorData(e);
                DependencyService.Get<ILocationManager>().StartLocationUpdates();
            }
            else if (Device.RuntimePlatform == Device.iOS){
                ProcessSensorData(e);
            }


        }

        void CreateHttpClient(int bufferSize){
            client = new HttpClient();
            client.MaxResponseContentBufferSize = bufferSize;
        }

        async void ProcessSensorData(LocationEventArgs args){
            var response = await client.GetAsync(new Uri("http://mobiheaven.de/digital-city/index.php?latitude=1&longitude=2&radius=1"));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                ProcessRoadSensor(collection.roadSensors[random.Next() % collection.roadSensors.Length], args);
                ProcessTraficLaneSensor(collection.laneSensors[random.Next() % collection.laneSensors.Length], args);
                ProcessTrafficLightSensor(collection.lightSensors[random.Next() % collection.lightSensors.Length], args);
                ProcessEnvirontmentalSensor(collection.envSensors[random.Next() % collection.envSensors.Length], args);
            }
            else{
                DependencyService.Get<INotification>().SendDefaultNotification(999, "Warning", "There is an issue with the connection");
            }
        }

        void ProcessEnvirontmentalSensor(Model.EnviromentalSensor sensor, LocationEventArgs args){
            if (IsSensorInRange(sensor, args, 500))
            {
                if (sensor.activeOxygen > 200)
                    DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.OxygenPollution, "Pollution warning", "Oxygen has exceeded the threshold of 200 ppm");
                if(sensor.carbonMonoxidePPM > 200)
                    DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon monoxide has exceeded the threshold of 200 ppm");
                if(sensor.carbonDioxidePPM > 200)
                    DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon dioxide has exceeded the threshold of 200 ppm");
            }
        }

        void ProcessRoadSensor(Model.RoadSensor sensor, LocationEventArgs args){
             if (IsSensorInRange(sensor, args, 500))
             {
                 string content = string.Format("The outdoor temperature is {0} degres. ", sensor.currentEnviromentTemperature);

                 string imagePath = null;

                 if (sensor.humidityLevel > 20.0)
                 {
                    imagePath = "shower3";
                 }
                 else if(sensor.currentEnviromentTemperature > 20.0)
                 {
                    imagePath = "sunny";
                 }
                 else{
                    imagePath = "cloudy2";
                 }

                 DependencyService.Get<INotification>().SendCollapsedNotification((int)Model.Notification.Type.Weather, "Weather", content, imagePath);

            }
        }

        void ProcessTrafficLightSensor(Model.TrafficLightSensor sensor, LocationEventArgs args){
                if (IsSensorInRange(sensor, args, 500)){
                    if(sensor.trafficLightState.Equals("red")){
                        DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is red.");
                    }
                    else if(sensor.trafficLightState.Equals("green")){
                        DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is green.");
                    }
                }
        }

        void ProcessTraficLaneSensor(Model.TrafficLaneSensor sensor, LocationEventArgs args){
                if (IsSensorInRange(sensor, args, 500)){
                    if(sensor.crossingsCounter5Minutes < 20 && sensor.averageSpeed < 40){
                        DependencyService.Get<INotification>().SendDefaultNotification((int)Model.Notification.Type.TrafficJam, "Traffic jam", "There is a traffic jam ahead.");
                    }
                }
        }

        bool IsSensorInRange(Model.Sensor sensor, LocationEventArgs args, double threshold){
            double distance = Math.Sqrt(Math.Pow(sensor.longitude - args.Longitude, 2) + Math.Pow(sensor.latitude - args.Latitude, 2));
            return distance <= threshold;
        }
	}
}
