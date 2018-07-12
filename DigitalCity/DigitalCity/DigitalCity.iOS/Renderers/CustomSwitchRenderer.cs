using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalCity.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Switch), typeof(CustomSwitchRenderer))]
namespace DigitalCity.iOS
{
    public class CustomSwitchRenderer : SwitchRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);

            Control.OnTintColor = Color.FromHex("#39ff14").ToUIColor();
            Control.TintColor = UIColor.White;
        }
    }
}