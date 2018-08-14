using System;
using DigitalCity.iOS;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationiOS))]
namespace DigitalCity.iOS
{
    public class NotificationiOS : INotification
    {
        /*
         * Empty Contructor
         */
        public NotificationiOS()
        {
            
        }

        /*
         * Delete an pending or delivered notification
         */
        public void DeleteNotification(int id)
        {
            string[] notifyID = { Convert.ToString(id) };
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(notifyID);
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(notifyID);
        }

        /*
         * Ask for Permission to receive notifications
         */
        public void GetPermissions()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => { });
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

            var UNContent = CreateContent(title, content);
            UNContent = AddImageAttachment(UNContent, image);
            PublishNotification(id, UNContent);
        }

        /*
         * Submit default notification
         */ 
        public void SendDefaultNotification(DigitalCity.Model.Notification notification)
        {
            var title = notification.title;
            var content = notification.content;
            var image = notification.imagePath;
            var id = notification.GetId();

            var UNcontent = CreateContent(title, content);
            PublishNotification(id, UNcontent);
        }

        /*
         * Link up image with notification
         */ 
        private UNMutableNotificationContent AddImageAttachment(UNMutableNotificationContent content, string image){
            var localUrl = "file://" + NSBundle.MainBundle.PathForResource(image, "png");
            NSUrl url = NSUrl.FromString(localUrl);
            var options = new UNNotificationAttachmentOptions();
            NSError error;
            var attachment = UNNotificationAttachment.FromIdentifier("image", url, options, out error);
            content.Attachments = new UNNotificationAttachment[] { attachment };

            return content;
        }

        /*
         * Create Context including title and body for notification
         */
        private UNMutableNotificationContent CreateContent(string title, string content){
            UNMutableNotificationContent UNContent = new UNMutableNotificationContent();
            UNContent.Title = title;
            UNContent.Body = content;

            return UNContent;
        }

        /*
         * Publish notification to the notification center
         */
        private void PublishNotification(int id, UNMutableNotificationContent content){
            UNNotificationRequest request = UNNotificationRequest.FromIdentifier(Convert.ToString(id), content, null);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => { });
        }

    }
}
