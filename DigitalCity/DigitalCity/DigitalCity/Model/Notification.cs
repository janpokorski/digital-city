using System;
namespace DigitalCity.Model
{
    public class Notification
    {
        public enum Type {Weather, RoadCondition, TrafficLight, TrafficJam, OxygenPollution, CarbonDioxidePollution, CarbonMonoxidePollution, Issue}

        public Type type;
        public string title;
        public string content;
        public string imagePath = null;
        public int priority = 0;

        public Notification(Type type, string title, string content, int priority)
        {
            this.type = type;
            this.title = title;
            this.content = content;
            this.priority = priority;
        }

        public Notification(Type type, string title, string content, string imagePath, int priority){
            this.type = type;
            this.title = title;
            this.content = content;
            this.imagePath = imagePath;
            this.priority = priority;
        }

        public int GetId()
        {
            return (int)type;
        }

        public int GetId(Type type)
        {
            return (int)type;
        }

    }
}
