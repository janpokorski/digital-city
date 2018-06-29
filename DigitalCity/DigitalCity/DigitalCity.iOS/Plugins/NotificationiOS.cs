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
        public NotificationiOS()
        {
            
        }

        public void DeleteNotification(int id)
        {
            string[] notifyID = { Convert.ToString(id) };
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(notifyID);
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(notifyID);
        }

        public void GetPermissions()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => { });
        }

        public void SendCollapsedNotification(int id, string title, string content, string image)
        {
            var UNContent = CreateContent(title, content);
            UNContent = AddImageAttachment(UNContent, image);
            PublishNotification(id, UNContent);
        }

        public void SendDefaultNotification(int id, string title, string content)
        {
            var UNcontent = CreateContent(title, content);
            PublishNotification(id, UNcontent);
        }

        private UNMutableNotificationContent AddImageAttachment(UNMutableNotificationContent content, string image){
            var localUrl = "file://" + NSBundle.MainBundle.PathForResource(image, "png");
            NSUrl url = NSUrl.FromString(localUrl);
            var options = new UNNotificationAttachmentOptions();
            NSError error;
            var attachment = UNNotificationAttachment.FromIdentifier("image", url, options, out error);
            content.Attachments = new UNNotificationAttachment[] { attachment };

            return content;
        }

        private UNMutableNotificationContent CreateContent(string title, string content){
            UNMutableNotificationContent UNContent = new UNMutableNotificationContent();
            UNContent.Title = title;
            UNContent.Body = content;

            return UNContent;
        }

        private void PublishNotification(int id, UNMutableNotificationContent content){
            UNNotificationRequest request = UNNotificationRequest.FromIdentifier(Convert.ToString(id), content, null);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => { });
        }

    }
}
