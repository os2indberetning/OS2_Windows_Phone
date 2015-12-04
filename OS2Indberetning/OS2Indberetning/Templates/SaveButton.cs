using System;
using System.Collections.Generic;
using System.Text;
using OS2Indberetning;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Templates
{
    public class SaveButton : ContentView
    {
        private Label _label;
        private StackLayout _layout;

        /// <summary>
        /// Creates a new instance of an animated + button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public SaveButton(Action callback = null)
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 10, 5, 0),
                WidthRequest = Definitions.HeaderHeight,
            };

            // create the label
            _label = new Label
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.HeaderFontSize - 7,
                Text = "Save",
                TextColor = Color.FromHex(Definitions.TextColor),
                //YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            _layout.Children.Add(_label);

            // add a gester reco
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async (o) =>
                {
                    await this.ScaleTo(0.95, 50, Easing.CubicOut);
                    await this.ScaleTo(1, 50, Easing.CubicIn);
                    if (callback != null)
                        callback.Invoke();
                })
            });

            // set the content
            this.Content = _layout;
        }
    }
}
