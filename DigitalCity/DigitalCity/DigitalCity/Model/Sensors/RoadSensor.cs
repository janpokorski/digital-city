using System;
namespace DigitalCity.Model
{
    /*
     *  This is a representation of the road sensor from the backend 
     */
    public class RoadSensor : Sensor
    {
        public string RoadSensorname;
        public string RoadSensornamespaceURI;
        public string RoadSensoridentifier;

        public double currentEnviromentTemperature;
        public double deicerDensity;
        public double dewPoint;
        public string freezingTemperature;
        public double friction;
        public double humidityLevel;
        public double icePercentage;
        public double relativeHumidityAtRoadTemperature;
        public double waterFilmHeight;
        public double waterFilmHeightOnSurface;
    }
}
