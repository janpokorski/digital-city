using System;
namespace DigitalCity.Model
{
    public class Notification
    {
        public enum Type {Weather, RoadCondition, TrafficLight, TrafficJam, OxygenPollution, CarbonDioxidePollution, CarbonMonoxidePollution}

        public Type type;
        public string sensorIdentifier;
        public string title;
        public string content;
        public string imagePath;

        public Notification(Type type, string sensorIdentifier, string title, string content, string imagePath){
            this.type = type;
            this.sensorIdentifier = sensorIdentifier;
            this.title = title;
            this.content = content;
            this.imagePath = imagePath;
        }

    }
}
