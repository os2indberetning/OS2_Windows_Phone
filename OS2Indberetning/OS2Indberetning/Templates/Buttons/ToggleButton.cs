using System;
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class ToggleButton : ContentView
    {
        private Label _textLabel;
        private StackLayout _layout;
        private string _text;
        private string _text1;
        private string _text2;
        private bool _toggle;

        /// <summary>
        /// Creates a new instance of the animation button
        /// </summary>
        /// <param name="text1">the text to set when not pressed yet</param>
        /// /// <param name="text2">the text to set when pressed once</param>
        /// /// /// <param name="text3">the text to set when pressed twice</param>
        /// <param name="callback">action to call when the animation is complete</param>
        public ToggleButton(string text1, string text2, string text3, Action callback = null)
        {
            _text = text1;
            _text1 = text2;
            _text2 = text3;
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
                Text = _text,
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
                    _toggle = !_toggle;
                    Text = _toggle ? _text1 : _text2;
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
        /// Gets or sets the text for the button
        /// </summary>
        public virtual string Text
        {
            get
            {
                return _textLabel.Text;
            }
            set
            {
                _textLabel.Text = value;
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
    }
}
