using System;
namespace DigitalCity.Model.Notifications
{
    public class Notification
    {
        public enum Type { Weather, Pollution, RoadCondition, TrafficLight, TrafficJam };

        public Type type;
        public string sensorIdentifier;
        public string title;
        public string content;
        public string largeImagePath;

        public Notification(Type type){
            this.type = type;
        }
    }
}
