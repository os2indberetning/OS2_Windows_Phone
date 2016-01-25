/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
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
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Padding = 5,
                Spacing = -1,
                WidthRequest = Definitions.HeaderHeight,
            };

            if (stored != "0")
            {
                _label = new Label
                {
                    Text = stored,
                    FontFamily = Definitions.FontFamily,
                    FontSize = Definitions.HeaderFontSize - 25,
                    TextColor = Color.FromHex(Definitions.TextColor),
                    VerticalOptions = LayoutOptions.End,
                };
                _layout.Children.Add(_label);
            }

            _image = new Image
            {
                Source = "Resources/verts2.png",
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
    }
}
