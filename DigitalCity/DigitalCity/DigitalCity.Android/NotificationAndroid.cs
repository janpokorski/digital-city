using System;
using Android.App;
using DigitalCity.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationAndroid))]
namespace DigitalCity.Droid
{
    public class NotificationAndroid : INotification
    {
        private NotificationManager manager;

        public NotificationAndroid()
        {
            var context = CrossCurrentActivity.Current.AppContext.ApplicationContext;
            manager = context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;
        }

        public void GetPermissions()
        {
            
        }

        public void SetNotification(string title, string content)
        {
            Notification.Builder builder = new Notification.Builder(CrossCurrentActivity.Current.Activity);
            builder.SetContentTitle(title);
            builder.SetContentText(content);
            builder.SetSmallIcon(Resource.Drawable.icon);

            Notification notification = builder.Build();

            manager.Notify(0, notification);

        }
    }
}
