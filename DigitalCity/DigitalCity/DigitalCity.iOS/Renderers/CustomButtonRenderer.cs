using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalCity.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace DigitalCity.iOS
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            Control.Layer.BorderWidth = 1;
            Control.SetTitleColor(UIColor.White, UIControlState.Normal);
            Control.SetTitleColor(Color.FromHex("#39ff14").ToUIColor(), UIControlState.Highlighted);
            Control.Layer.BorderColor = Color.White.ToCGColor();
        }
    }
}