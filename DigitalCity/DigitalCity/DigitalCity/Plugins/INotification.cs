using DigitalCity.Model;
using System;
namespace DigitalCity
{

    public interface INotification
    {
        
        void GetPermissions();
        void SendDefaultNotification(Notification notification);
        void SendCollapsedNotification(Notification notification);
        void DeleteNotification(int id);
    }
}
