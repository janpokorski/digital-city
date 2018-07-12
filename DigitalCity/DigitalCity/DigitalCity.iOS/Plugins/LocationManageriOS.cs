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

        public LocationManageriOS()
        {
            manager = new CLLocationManager();
            manager.PausesLocationUpdatesAutomatically = false;
            manager.AllowsBackgroundLocationUpdates = true;

        }

        public event LocationUpdatedEventHandler LocationUpdated;

        public virtual void LocationHandler(object sender, CLLocationsUpdatedEventArgs e)
        {
            if(LocationUpdated != null)
            {
                LocationEventArgs arg = new LocationEventArgs(e.Locations[e.Locations.Length - 1].Coordinate.Longitude, e.Locations[e.Locations.Length - 1].Coordinate.Latitude);
                LocationUpdated(this, arg);
            }
        }

        public void GetPermissions()
        {
            manager.RequestWhenInUseAuthorization();
            manager.RequestAlwaysAuthorization();
        }

        public void StartLocationUpdates()
        {
            manager.DesiredAccuracy = CLLocation.AccuracyBest;
            manager.LocationsUpdated += LocationHandler;
            manager.StartUpdatingLocation();
        }

        public void StopLocationUpdates()
        {
            manager.StopUpdatingLocation();
        }

    }
}
