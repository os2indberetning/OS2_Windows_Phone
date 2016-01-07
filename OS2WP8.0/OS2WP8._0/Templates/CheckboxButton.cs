/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class CheckboxButton : ContentView
    {
        private Image _image;
        private StackLayout _layout;
        private bool _toggle;
        private ImageSource _img_unchecked = "Resources/checkbox_unchecked.png";
        private ImageSource _img_checked = "Resources/checkbox_checked.png";


        /// <summary>
        /// Creates a new instance of the animation button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public CheckboxButton(Action callback = null, bool toggle = false)
        {
            _toggle = toggle;
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(0, 3, 0, 0),
                Orientation = StackOrientation.Horizontal,
                HeightRequest = Definitions.ButtonHeight,
            };
            if (_toggle)
            {
                _image = new Image
                {
                    Source = _img_checked,
                    HeightRequest = 42,
                    WidthRequest = 43,
                };
            }
            else
            {
                _image = new Image
                {
                    Source = _img_unchecked,
                    HeightRequest = 42,
                    WidthRequest = 43,
                };
            }
            _layout.Children.Add(_image);


            // add a gester reco
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async (o) =>
                {
                    if (callback != null)
                    {
                        _toggle = !_toggle;
                        Source = _toggle ? _img_checked : _img_unchecked;
                        await this.ScaleTo(0.95, 50, Easing.CubicOut);
                        await this.ScaleTo(1, 50, Easing.CubicIn);
                        callback.Invoke();
                    }
                        
                })
            });

            // set the content
            this.Content = _layout;
        }



        /// <summary>
        /// Gets or sets the text for the button
        /// </summary>
        public virtual ImageSource Source
        {
            get
            {
                return _image.Source;
            }
            set
            {
                _image.Source = value;
            }
        }
    }
}
