using System;
namespace DigitalCity.Model
{
    /*
     *  This static class stores wheter some notifications should or shoudn't be displayed
     */
    public static class EnabledNotifications
    {
        public static bool weather = true;
        public static bool pollution = true;
        public static bool lights = true;
        public static bool jams = true;
    }
}
