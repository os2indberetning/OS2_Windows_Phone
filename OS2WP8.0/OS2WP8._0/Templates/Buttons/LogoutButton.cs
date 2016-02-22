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
    public class LogoutButton : ContentView
    {
        private Image _image;
        private StackLayout _layout;

        /// <summary>
        /// Creates a new instance of an animated logout button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public LogoutButton(Action callback = null)
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Rotation = 180,
                Padding = new Thickness(16, 0, -16, 0),
                HeightRequest = Definitions.HeaderHeight,
                WidthRequest = Definitions.HeaderHeight
            };

            // create the label
            _image = new Image
            {
                Source = "Resources/exit.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Scale = 0.6
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
