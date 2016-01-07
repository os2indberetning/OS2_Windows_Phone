/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class ButtomButton : ContentView
    {
        private Label _textLabel;
        private StackLayout _layout;

        /// <summary>
        /// Creates a new instance of the animation button
        /// </summary>
        /// <param name="text">the text to set</param>
        /// <param name="callback">action to call when the animation is complete</param>
        public ButtomButton(string text, Action callback = null)
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex(Definitions.SecondaryColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                HeightRequest = Definitions.ButtonHeight,
            };

            // create the label
            _textLabel = new Label
            {
                FontSize = Definitions.ButtonFontSize,
                Text = text,
                TextColor = Color.FromHex(Definitions.TextColor),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Center,
            };
            _layout.Children.Add(_textLabel);

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

        /// <summary>
        /// Gets or sets the font size for the text
        /// </summary>
        public virtual double FontSize
        {
            get { return this._textLabel.FontSize; }
            set
            {
                this._textLabel.FontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the text color for the text
        /// </summary>
        public virtual Color TextColor
        {
            get
            {
                return _textLabel.TextColor;
            }
            set
            {
                _textLabel.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the height for the button
        /// </summary>
        public virtual double Height
        {
            get
            {
                return _layout.Height;
            }
            set
            {
                _layout.HeightRequest = value;
            }
        }

        /// <summary>
        /// Gets or sets the width for the button
        /// </summary>
        public virtual double Width
        {
            get
            {
                return _layout.Width;
            }
            set
            {
                _layout.WidthRequest = value;
            }
        }
    }
}
