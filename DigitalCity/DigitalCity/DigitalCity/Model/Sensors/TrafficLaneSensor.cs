using System;
namespace DigitalCity.Model
{
    /*
     *  This is a representation of the traffic lane sensor from the backend 
     */
    public class TrafficLaneSensor : Sensor
    {
        public string TrafficLaneSensorname;
        public string TrafficLaneSensornamespaceURI;
        public string TrafficLaneSensoridentifier;
        public int crossingsCounter5Minutes;

        public double averageSpeed;
        public int crossingsCounter1Hour;
    }
}
