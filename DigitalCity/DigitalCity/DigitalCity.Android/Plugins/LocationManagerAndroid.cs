using System;
using Android;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using DigitalCity.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationManagerAndroid))]
namespace DigitalCity.Droid
{
    /*
     * Callback class used to get location from the LocationManager
     */
    public class ClientLocationCallback : LocationCallback
    {
        public ClientLocationCallback(Activity activity)
        {

        }

        /*
         * Called if location service gets unavailable
         */
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            if (!locationAvailability.IsLocationAvailable)
            {
                LocationManagerAndroid.Manager.OnLocationUpdate(null);
            }
        }

        /*
         * Called if there is a new location update
         */
        public override void OnLocationResult(LocationResult result)
        {
            LocationManagerAndroid.Manager.OnLocationUpdate(result);
        }
    }

    public class LocationManagerAndroid : ILocationManager
    {
        private FusedLocationProviderClient client;

        public static LocationManagerAndroid Manager { get; private set; }

        ClientLocationCallback locationCallback;

        public LocationManagerAndroid()
        {
            Manager = this;
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(CrossCurrentActivity.Current.AppContext);

            //check if Google Service for Location updates is available
            if (queryResult == ConnectionResult.Success)
            {
                return;
            }
            else if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Android.Widget.Toast.MakeText(CrossCurrentActivity.Current.AppContext, errorString, Android.Widget.ToastLength.Long).Show();

            }
        }

        /*
         * events that is fired once there is an update of location
         */
        public event LocationUpdatedEventHandler LocationUpdated;

        /*
        * Start receiving location updates
        */
        void ILocationManager.StartLocationUpdates()
        {
            client = LocationServices.GetFusedLocationProviderClient(CrossCurrentActivity.Current.Activity);
            LocationRequest request = new LocationRequest().SetPriority(LocationRequest.PriorityHighAccuracy).SetInterval(1000 * 5).SetFastestInterval(1000 * 2);
            locationCallback = new ClientLocationCallback(CrossCurrentActivity.Current.Activity);
            client.RequestLocationUpdatesAsync(request, locationCallback);
        }

        /*
         * Redirect location updates to the event
         */
        public void OnLocationUpdate(LocationResult result)
        {
            if(result == null)
            {
                LocationUpdated?.Invoke(this, null);
                return;
            }
            LocationUpdated?.Invoke(this, new LocationEventArgs(result.LastLocation.Longitude, result.LastLocation.Latitude));
        }

        /*
        * Stop updating location information
        */
        public void StopLocationUpdates()
        {
            client.RemoveLocationUpdatesAsync(locationCallback);
        }

        /*
         * Ask for permission to use location service
         */
        public void GetPermissions()
        {
            if (ContextCompat.CheckSelfPermission(CrossCurrentActivity.Current.AppContext, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Denied)
            {
                ActivityCompat.RequestPermissions(CrossCurrentActivity.Current.Activity, new String[] { Manifest.Permission.AccessFineLocation }, 10);
            }
        }
    }

}
