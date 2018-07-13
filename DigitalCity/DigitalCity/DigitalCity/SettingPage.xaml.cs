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
            weatherToggle.IsToggled = EnabledNotifications.weather;
            pollutionToggle.IsToggled = EnabledNotifications.pollution;
            lightToggle.IsToggled = EnabledNotifications.lights;
            jamToggle.IsToggled = EnabledNotifications.jams;
        }

        void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            EnabledNotifications.weather = weatherToggle.IsToggled;
        }

        void Handle_Toggled_1(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            EnabledNotifications.pollution = pollutionToggle.IsToggled;
        }

        void Handle_Toggled_2(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            EnabledNotifications.lights = lightToggle.IsToggled;
        }

        void Handle_Toggled_3(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            EnabledNotifications.jams = jamToggle.IsToggled;
        }
    }
}
