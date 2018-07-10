using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalCity.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace DigitalCity.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            
            Control.TextColor = UIColor.White;
            Control.BackgroundColor = UIColor.FromRGBA(0,0,0,0.2f);
            Control.TintColor = Color.FromHex("#39ff14").ToUIColor();
        }
    }
}