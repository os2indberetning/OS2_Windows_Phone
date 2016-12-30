/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using OS2WP8._0.Platform;
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class VertsButton : ContentView
    {
        private Image _image;
        private StackLayout _layout;
        private Label _label;

        /// <summary>
        /// Creates a new instance of an animated + button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public VertsButton(Action callback = null, string stored = "0")
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(-20, 5, 20, 5),
                WidthRequest = Definitions.HeaderHeight,
                
            };
            var circleButton = new MyButton
            {
                Text = stored,
                BorderColor = Color.FromHex(Definitions.TextColor),
                TextColor = Color.FromHex(Definitions.PrimaryColor),
                BackgroundColor = Color.FromHex(Definitions.TextColor),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                AnchorY = (double)TextAlignment.Center,
                FontSize = 14,
                BorderRadius = 30,
                HeightRequest = 60,
                WidthRequest = 60,
            };

            var _inner = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(13, 0, -13, 0)
            };

            _image = new Image
            {
                Source = "Resources/verts2.png",
                HorizontalOptions = LayoutOptions.EndAndExpand,

            };
            
            _inner.Children.Add(circleButton);
            _layout.Children.Add(_inner);
            _layout.Children.Add(_image);

            if (stored == "0")
            {
                _layout.Padding = new Thickness(-25, 5, 25, 5);
                circleButton.Opacity = 0;
            }

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
