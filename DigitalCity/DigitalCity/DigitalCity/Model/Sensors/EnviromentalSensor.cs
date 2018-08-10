using System;
namespace DigitalCity.Model
{
    /*
     *  This is a representation of the environmental sensor from the backend
     */
    public class EnviromentalSensor : Sensor
    {
        public string EnvironmentalSensorname;
        public string EnvironmentalSensornamespaceURI;
        public string EnvironmentalSensoridentifier;

        public double activeOxygen;
        public double carbonDioxidePPM;
        public double carbonMonoxidePPM;
        public double humidityLevel;
        public double particlePM1;
        public double particlePM10;
        public double particlePM2dot5;
    }
}
