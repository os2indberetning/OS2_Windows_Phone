using System.Linq;
using System.Windows;
using System.Windows.Media;
using CustomRenderer.WinPhone81;
using Microsoft.Phone.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using Thickness = System.Windows.Thickness;

[assembly: ExportRenderer(typeof(Entry), typeof(MyEntryRenderer))]
namespace CustomRenderer.WinPhone81
{
    public class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            
            if (Control != null)
            {
               
                var borders = Control.Children.OfType<PhoneTextBox>();
                foreach (var each in borders)
                {
                    each.BorderBrush = new SolidColorBrush(Colors.Black);
                    each.BorderThickness = new Thickness(1);
                    each.HorizontalContentAlignment = HorizontalAlignment.Center;
                    
                }
            }
        }
    }
}
