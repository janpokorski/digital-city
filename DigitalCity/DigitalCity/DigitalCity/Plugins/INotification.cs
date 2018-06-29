using System;
namespace DigitalCity
{

    public interface INotification
    {
        
        void GetPermissions();
        void SendDefaultNotification(int id, string title, string content);
        void SendCollapsedNotification(int id, string title, string content, string image);
        void DeleteNotification(int id);
    }
}
