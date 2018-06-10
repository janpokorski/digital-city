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
    public class ClientLocationCallback : LocationCallback
    {
        public ClientLocationCallback(Activity activity)
        {

        }
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            if (!locationAvailability.IsLocationAvailable)
            {
                LocationManagerAndroid.Manager.OnLocationUpdate(null);
            }
        }

        public override void OnLocationResult(LocationResult result)
        {
            LocationManagerAndroid.Manager.OnLocationUpdate(result);
        }
    }

    public class LocationManagerAndroid : ILocationManager
    {
        private FusedLocationProviderClient client;

        public static LocationManagerAndroid Manager { get; private set; }

        public LocationManagerAndroid()
        {
            Manager = this;
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(CrossCurrentActivity.Current.AppContext);
            if (queryResult == ConnectionResult.Success)
            {
                return;
            }
            else if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);

                //Show alert
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(CrossCurrentActivity.Current.AppContext);
                alert.SetTitle("Warning");
                alert.SetMessage(string.Format("There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString));
                alert.SetNeutralButton("Ok", delegate {
                    alert.Dispose();
                });
                alert.Show();

            }
        }

        public event LocationUpdatedEventHandler LocationUpdated;

        void ILocationManager.StartLocationUpdates()
        {
            client = LocationServices.GetFusedLocationProviderClient(CrossCurrentActivity.Current.Activity);
            LocationRequest request = new LocationRequest().SetPriority(LocationRequest.PriorityHighAccuracy).SetInterval(500).SetFastestInterval(100);
            client.RequestLocationUpdatesAsync(request, new ClientLocationCallback(CrossCurrentActivity.Current.Activity));
        }

        public void OnLocationUpdate(LocationResult result)
        {
            if(result == null)
            {
                LocationUpdated?.Invoke(this, null);
                return;
            }
            LocationUpdated?.Invoke(this, new LocationEventArgs(result.LastLocation.Longitude, result.LastLocation.Latitude));
        }

        public void StopLocationUpdates()
        {
            throw new NotImplementedException();
        }

        public void GetPermissions()
        {
            if (ContextCompat.CheckSelfPermission(CrossCurrentActivity.Current.AppContext, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Denied)
            {
                ActivityCompat.RequestPermissions(CrossCurrentActivity.Current.Activity, new String[] { Manifest.Permission.AccessFineLocation }, 10);
            }
        }
    }

}
