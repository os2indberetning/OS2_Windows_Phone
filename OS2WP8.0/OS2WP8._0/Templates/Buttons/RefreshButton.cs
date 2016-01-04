using System;
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class RefreshButton : ContentView
    {
        private Image _image;
        private StackLayout _layout;

        /// <summary>
        /// Creates a new instance of an animated + button
        /// </summary>
        /// <param name="callback">action to call when the animation is complete</param>
        public RefreshButton(Action callback = null)
        {
            // create the layout
            _layout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Padding = 5,
                WidthRequest = Definitions.HeaderHeight,
            };

            // create the label
            _image = new Image
            {
                Source = "Resources/refresh.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Rotation = Rotation + 90,
                Scale = 0.8
            };
            _layout.Children.Add(_image);

            // add a gester reco
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async (o) =>
                {
                    var timerContinue = 30;
                    Device.StartTimer(TimeSpan.FromSeconds(0.05), () =>
                    {
                        if (timerContinue > 0)
                        {
                            _image.Rotation = _image.Rotation + 24;
                            timerContinue--;
                            return true;
                        }
                        return false; //not continue
                    });
                    if (callback != null)
                        callback.Invoke();
                })
            });

            // set the content
            this.Content = _layout;
        }
    }
}
