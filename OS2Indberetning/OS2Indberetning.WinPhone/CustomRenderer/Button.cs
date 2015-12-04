using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using System.Windows.Media;
using CustomRenderer;
using XLabs.Forms.Controls;

// for Color and Colors


[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(CustomButtonRenderer))]
namespace CustomRenderer
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            var button = e.NewElement; // Xamarin.Forms.Button control

            //var groups = VisualStateManager.GetVisualStateGroups(this.Control);
            //var test = new VisualStateGroup();
            // button.Unfocus();
            //button.Focused += delegate(object sender, FocusEventArgs args) { Debug.WriteLine("boho"); };
            if (this.Control != null)
            {
                
                // Control refers to the instance of System.Windows.Controls.Button
                // created by the base renderer
                //byte a = byte.Parse(Definitions.SecondaryColor.Substring(1, 2),NumberStyles.HexNumber);
                //byte r = byte.Parse(Definitions.SecondaryColor.Substring(3, 2), NumberStyles.HexNumber);
                //byte g = byte.Parse(Definitions.SecondaryColor.Substring(5, 2), NumberStyles.HexNumber);
                //byte b = byte.Parse(Definitions.SecondaryColor.Substring(7, 2), NumberStyles.HexNumber);

                //VisualStateManager.GoToState(this.Control,"Normal", false);
                //var state = new VisualState();
                //state.SetValue(NameProperty, "Pressed");
                //var stuff = VisualStateManager.GetVisualStateGroups(this);
                //var tt = 1;

            }
        }

        //protected override void OnGotFocus(object sender, RoutedEventArgs args)
        //{
        //    VisualStateManager.GoToState(this.Control, "Normal", false);
        //}

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    VisualStateManager.GoToState(this.Control, "Normal", false);
        //}
    }
}
