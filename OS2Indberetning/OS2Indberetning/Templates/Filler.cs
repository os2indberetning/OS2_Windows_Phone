using System;
using System.Collections.Generic;
using System.Text;
using OS2Indberetning;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Templates
{
    public class Filler : ContentView
    {
        private Image _image;
        private StackLayout _layout;

        /// <summary>
        /// Creates a new instance of an animated + button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public Filler(Action callback = null)
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 0, 5, 0),
                WidthRequest = Definitions.HeaderHeight,
            };

            // create the label
            _image = new Image
            {
                Source = "Resources/add.png",
                Opacity = 0,
            };
            _layout.Children.Add(_image);

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

        public virtual double WidthRequest
        {
            get { return this._layout.WidthRequest; }
            set
            {
                this._layout.WidthRequest = value;
            }
        }
    }
}
