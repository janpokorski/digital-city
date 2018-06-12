using System;
namespace DigitalCity
{
    public interface INotification
    {
        void GetPermissions();
        void SetNotification(String title, String content);
    }
}
