using Android.App;
using Android.Graphics;
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
            
            //create head-up notification channel
            if(Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {

                //create Notification channel
                string channelID = "Location-based information";
                Java.Lang.ICharSequence channelName = new Java.Lang.String("Location-based information");
                NotificationChannel channel = new NotificationChannel(channelID, channelName, NotificationImportance.Max);

                //assign channel to the current manager
                var context = CrossCurrentActivity.Current.AppContext.ApplicationContext;
                manager = context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;
                manager.CreateNotificationChannel(channel);
            }

        }

        /*
         * Delete an pending or delivered notification
         */
        public void DeleteNotification(int id)
        {
            manager.Cancel(id);
        }

        /*
         * Empty because no need for notification permission
         */
        public void GetPermissions()
        {
            
        }

        /*
         * Submit notification with a small icon on the very right
         */
        public void SendCollapsedNotification(DigitalCity.Model.Notification notification)
        {
            var title = notification.title;
            var content = notification.content;
            var image = notification.imagePath;
            var id = notification.GetId();

            //create builder and assign image to the notification
            Notification.Builder builder = CreateNotificationBuilder(title, content);
            int imageID = (int)typeof(Resource.Drawable).GetField(image).GetRawConstantValue();
            builder.SetLargeIcon(BitmapFactory.DecodeResource(CrossCurrentActivity.Current.Activity.Resources, imageID));
            PublishNotification(builder, id);
        }

        /*
         * Submit default notification
         */
        public void SendDefaultNotification(DigitalCity.Model.Notification notification)
        {
            var title = notification.title;
            var content = notification.content;
            var id = notification.GetId();

            Notification.Builder builder = CreateNotificationBuilder(title, content);
            PublishNotification(builder, id);

        }

        /*
         * Submit user-interactive notification (not finished)
         */
        public void SendExpandedNotification(DigitalCity.Model.Notification notification)
        {
            var title = notification.title;
            var content = notification.content;
            var image = notification.imagePath;
            var id = notification.GetId();

            Notification.Builder builder = CreateNotificationBuilder(title, content);
            int imageID = (int)typeof(Resource.Drawable).GetField(image).GetRawConstantValue();
            builder.SetStyle(new Notification.BigPictureStyle().BigPicture(BitmapFactory.DecodeResource(CrossCurrentActivity.Current.Activity.Resources, imageID)));
            PublishNotification(builder, id);
        }

        /*
        * Create builder and assign title and text to the notification
        */
        public Notification.Builder CreateNotificationBuilder(string title, string content){
            Notification.Builder builder = null;

            //if version is lower than Android O, priority has to be set manually
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                builder = new Notification.Builder(CrossCurrentActivity.Current.Activity.ApplicationContext, "Location-based information");
            }
            else
            {
                builder = new Notification.Builder(CrossCurrentActivity.Current.Activity.ApplicationContext, "Location-based information");
                int priority = (int)NotificationPriority.High;
                builder.SetPriority(priority);
            }
            var context = CrossCurrentActivity.Current.AppContext;
            builder.SetContentTitle(title);
            builder.SetContentText(content);
            builder.SetSmallIcon(Resource.Drawable.Notification_Icon);
            return builder;
        }

        /*
         * Publish notification to the notification center
         */
        public void PublishNotification(Notification.Builder builder, int id){
            Notification notification = builder.Build();
            manager.Notify(id, notification);
        }
    }
}
