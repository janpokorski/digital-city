using System;
namespace DigitalCity.Model
{
    /*
     *  This is a collection of all supported sensors
     */
    public class SensorCollection
    {
        public EnviromentalSensor[] envSensors;
        public RoadSensor[] roadSensors;
        public TrafficLaneSensor[] laneSensors;
        public TrafficLightSensor[] lightSensors;
    }
}
