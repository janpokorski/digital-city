using System;
using CoreLocation;
using DigitalCity.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationManageriOS))]

namespace DigitalCity.iOS
{
    public class LocationManageriOS : ILocationManager
    {
        private CLLocationManager manager;

        /*
         * Contructor for LocationManager
         */
        public LocationManageriOS()
        {
            manager = new CLLocationManager();

            //make LocationManager run in the background without being interrupted
            manager.PausesLocationUpdatesAutomatically = false;
            manager.AllowsBackgroundLocationUpdates = true;

        }

        /*
         * events that is fired once there is an update of location
         */
        public event LocationUpdatedEventHandler LocationUpdated;

        /*
         * Handler for Location updates
         */
        public virtual void LocationHandler(object sender, CLLocationsUpdatedEventArgs e)
        {
            if(LocationUpdated != null)
            {
                //fire event LocationUpdated with the location information as parameters
                LocationEventArgs arg = new LocationEventArgs(e.Locations[e.Locations.Length - 1].Coordinate.Longitude, e.Locations[e.Locations.Length - 1].Coordinate.Latitude);
                LocationUpdated(this, arg);
            }
        }

        /*
         * Ask for permission to use location service
         */
        public void GetPermissions()
        {
            manager.RequestWhenInUseAuthorization();
            manager.RequestAlwaysAuthorization();
        }

        /*
         * Start receiving location updates
         */
        public void StartLocationUpdates()
        {
            manager.DesiredAccuracy = CLLocation.AccuracyBest;
            manager.LocationsUpdated += LocationHandler;
            manager.StartUpdatingLocation();
        }

        /*
         * Stop updating location information
         */
        public void StopLocationUpdates()
        {
            manager.StopUpdatingLocation();
        }

    }
}
