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
    /*
     * Code-behind for the main page
     * 
     * Functions:
     * - Retrieve data from the backend
     * - Process the data
     * - Display various notifications
     */
	public partial class MainPage : ContentPage
	{
        HttpClient client;

		public MainPage()
		{
			InitializeComponent();

            ToolbarItem item = null;

            //Sets the toolbar buttonfor the settings page
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

            //Asks for the permission for the notifications service
            DependencyService.Get<INotification>().GetPermissions();
            //Asks for the permission for the location service
            DependencyService.Get<ILocationManager>().GetPermissions();

            CreateHttpClient(50000);
        }

        /*
         *  When the toolbar button is tapped this function navigates the user to the setting page 
         */
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingPage());
        }

        /*
         * When the start button is tapped this function opens the destination in a third party navigation app
         * and starts the background location service
         */
        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            //Starts the background location service
            DependencyService.Get<ILocationManager>().StartLocationUpdates();
            DependencyService.Get<ILocationManager>().LocationUpdated += LocationUpdateHandler;

            //Gets the destination
            var address = inputEntry.Text;

            //Launches a third party navigation app with the destination as parameter
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

        /*
         * This function is called every gps position update 
         */
        void LocationUpdateHandler(object sender, LocationEventArgs e)
        {
            //Error handling
            if(e == null)
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the location service", 3));
                return;
            }

            //Android: Stops location service, retrieves sensor data form backend and starts the location service again
            if(Device.RuntimePlatform == Device.Android){
                DependencyService.Get<ILocationManager>().StopLocationUpdates();
                GetSensorData(e);
                DependencyService.Get<ILocationManager>().StartLocationUpdates();
            }
            //iOS: Retrieves sensord data from backend
            else if (Device.RuntimePlatform == Device.iOS){
                GetSensorData(e);
            }


        }

        /*
         * Creates a new Http client with a custom max buffer size
         */ 
        void CreateHttpClient(int bufferSize){
            client = new HttpClient();
            client.MaxResponseContentBufferSize = bufferSize;
        }

        /*
         * Retrieves sensor data from the backend for every supported sensor type
         */
        void GetSensorData(LocationEventArgs args){
            GetRoadSensorData(args);
            GetLaneSensorData(args);
            GetLightSensorData(args);
            GetEnvSensorData(args);
        }

        /*
         * Retrieves the road sensor data from the backend
         * Radius: 200m
         * Parameter: gps position
         */
        async void GetRoadSensorData(LocationEventArgs args)
        {
            //Enabled in settings?
            if (!EnabledNotifications.weather)
                return;
            
            //Backend script location
            string uri = "http://localhost:80/backend.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "200";

            uri += "&type=roadSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                //Converts the json object to a c# representation
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                //If any results then process a random sensor data
                if (collection.roadSensors.Length > 0)
                    ProcessRoadSensor(collection.roadSensors[random.Next(collection.roadSensors.Length)], args);
            }
            //Error handling
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        /*
         * Retrieves the lane sensor data from the backend
         * Radius: 500m
         * Parameter: gps position
         */
        async void GetLaneSensorData(LocationEventArgs args)
        {
            //Enabled in settings?
            if (!EnabledNotifications.jams)
                return;

            //Backend script location
            string uri = "http://localhost:80/backend.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";

            uri += "500";

            uri += "&type=laneSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                //Converts the json object to a c# representation
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                //If any results then process a random sensor data
                if (collection.laneSensors.Length > 0)
                    ProcessTraficLaneSensor(collection.laneSensors[random.Next(collection.laneSensors.Length)], args);
            }
            //Error handling
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        /*
         * Retrieves the light sensor data from the backend
         * Radius: 300m
         * Parameter: gps position
         */
        async void GetLightSensorData(LocationEventArgs args)
        {
            //Enabled in settings?
            if (!EnabledNotifications.lights)
                return;

            //Backend script location
            string uri = "http://localhost:80/backend.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "300";

            uri += "&type=lightSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                //Converts the json object to a c# representation
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                //If any results then process a random sensor data
                if (collection.lightSensors.Length > 0)
                    ProcessTrafficLightSensor(collection.lightSensors[random.Next(collection.lightSensors.Length)], args);
            }
            //Error handling
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        /*
         * Retrieves the environmental sensor data from the backend
         * Radius: 1000m
         * Parameter: gps position
         */
        async void GetEnvSensorData(LocationEventArgs args)
        {
            //Enabled in settings?
            if (!EnabledNotifications.pollution)
                return;

            //Backend script location
            string uri = "http://mobiheaven.de/digital-city/index.php?lat=" + args.Latitude.ToString().Replace(",", ".")
                + "&long=" + args.Longitude.ToString().Replace(",", ".") + "&radius=";
            
            uri += "1000";

            uri += "&type=envSensor";

            var response = await client.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                //Converts the json object to a c# representation
                var collection = JsonConvert.DeserializeObject<Model.SensorCollection>(body);

                Random random = new Random();

                //If any results then process a random sensor data
                if (collection.envSensors.Length > 0)
                    ProcessEnvirontmentalSensor(collection.envSensors[random.Next(collection.envSensors.Length)], args);
            }
            //Error handling
            else
            {
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.Issue, "Warning", "There is an issue with the connection", 3));
            }
        }

        /*
         * Process environmental sensor data
         * Active oxygen > 200ppm then warning (priority 1)
         * Carbon monoxide > 200 ppm then alert (priority 3)
         * Carbon dioxide > 200 ppm then warning (priority 2)
         */
        void ProcessEnvirontmentalSensor(Model.EnviromentalSensor sensor, LocationEventArgs args){
             if (sensor.activeOxygen > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Oxygen has exceeded the threshold of 200 ppm", 1));
             if (sensor.carbonMonoxidePPM > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon monoxide has exceeded the threshold of 200 ppm", 3));
             if (sensor.carbonDioxidePPM > 200)
                DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.OxygenPollution, "Pollution warning", "Carbon dioxide has exceeded the threshold of 200 ppm", 2));
        }

        /*
         * Process road sensor data
         * Outside temperature <= 0 then show informational notification with snowy icon (priority 0)
         * Humidity level between 40 and 60 then show informational notification with cloudy icon (priority 0)
         * Humidity level > 60 then show informational notification with rainy icon (priority 0)
         * In other case show an informational notification with sunny icon (priority 0)
         */
        void ProcessRoadSensor(Model.RoadSensor sensor, LocationEventArgs args){
                //Displays the outside temperature
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
                 
                //Show notification
                DependencyService.Get<INotification>().SendCollapsedNotification(notification);
        }

        /*
         * Process traffic light sensor data
         * Displays a warning (priority 1) with the current traffic light state (red or green)
         */
        void ProcessTrafficLightSensor(Model.TrafficLightSensor sensor, LocationEventArgs args){
                if(sensor.trafficLightState.Equals("red")){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is red.", 1));
                }
                    else if(sensor.trafficLightState.Equals("green")){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficLight, "Traffic light", "Traffic light is green.", 1));
                }
        }

        /*
         * Process traffic lane sensor data
         * Displays a warning (priority 2) when high traffic and average speed < 40 km/h
         */
        void ProcessTraficLaneSensor(Model.TrafficLaneSensor sensor, LocationEventArgs args){
                if(sensor.crossingsCounter5Minutes < 20 && sensor.averageSpeed < 40){
                    DependencyService.Get<INotification>().SendDefaultNotification(new Model.Notification(Model.Notification.Type.TrafficJam, "Traffic jam", "There is a traffic jam ahead.", 2));
                }
        }
	}
}
