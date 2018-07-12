using System;
using System.Collections.Generic;
using DigitalCity.Model;
using Xamarin.Forms;

namespace DigitalCity
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        void toggled_weather(object sender, ToggledEventArgs args)
        {
            EnabledNotifications.weather = weatherToggle.IsToggled;
        }

        void toggled_pollution(object sender, ToggledEventArgs args)
        {
            EnabledNotifications.pollution = pollutionToggle.IsToggled;
        }

        void toggled_lights(object sender, ToggledEventArgs args)
        {
            EnabledNotifications.lights = lightToggle.IsToggled;
        }

        void toggled_jams(object sender, ToggledEventArgs arg)
        {
            EnabledNotifications.jams = jamToggle.IsToggled;
        }
    }
}
