using System;
namespace DigitalCity
{
    public class LocationEventArgs : EventArgs
    {
        public double Longitude;
        public double Latitude;

        public LocationEventArgs(double longitude, double latitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

    }

    public delegate void LocationUpdatedEventHandler(object sender, LocationEventArgs eventArgs);

    public interface ILocationManager
    {
        void GetPermissions();
        void StartLocationUpdates();
        void StopLocationUpdates();

        event LocationUpdatedEventHandler LocationUpdated;

    }
}
