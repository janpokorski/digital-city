using System;
using DigitalCity.iOS;
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

        public void GetPermissions()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => { });
        }

        public void SetNotification(string title, string content)
        {
            var UNContent = new UNMutableNotificationContent();
            UNContent.Title = title;
            UNContent.Body = content;

            var request = UNNotificationRequest.FromIdentifier("0", UNContent, null);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => {});
        }
    }
}
