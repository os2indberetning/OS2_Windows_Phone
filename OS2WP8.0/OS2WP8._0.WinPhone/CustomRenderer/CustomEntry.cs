/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomRenderer.WinPhone81;
using Microsoft.Phone.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using XLabs;
using TextAlignment = System.Windows.TextAlignment;
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
                //var hintStyle = new System.Windows.Style(typeof(ContentControl));
                ////var hintStyle = new System.Windows.Setter(PhoneTextBox.TextAlignmentProperty, System.Windows.TextAlignment.Center);
                ////var hintStyle2 = new System.Windows.Setter(PhoneTextBox.HintStyleProperty, hintStyle);
                //hintStyle.Setters.Add(new System.Windows.Setter(TextBox.HorizontalContentAlignmentProperty, System.Windows.TextAlignment.Center));
                //var test = new System.Windows.Style();
                //test.Setters.Add(hintStyle2); 
                //hintStyle.Setters.Add(new System.Windows.Setter(TextBox.TextAlignmentProperty, System.Windows.TextAlignment.Center));
                
                var borders = Control.Children.OfType<PhoneTextBox>();
                foreach (var each in borders)
                {
                    //each.HintStyle = hintStyle;
                    //each.ApplyTemplate();
                    each.TextAlignment = TextAlignment.Center;
                    each.BorderBrush = new SolidColorBrush(Colors.Black);
                    each.BorderThickness = new Thickness(1);
                    each.HorizontalContentAlignment = HorizontalAlignment.Center;
                    
                }
            }
        }
    }
}
