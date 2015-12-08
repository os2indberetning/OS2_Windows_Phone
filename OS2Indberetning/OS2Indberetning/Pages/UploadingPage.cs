using System.Collections.Generic;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning
{
    public class UploadingPage : ContentPage
    {
        // used to pop back to mainpage from viewmodel
        public IReadOnlyList<Page> NavigationStack;
        public INavigation Nav;

        public UploadingPage()
        {
            NavigationStack = Navigation.NavigationStack;
            Nav = Navigation;
            
            this.Content = this.SetContent();
        }

        public View SetContent()
        {
            var emblem = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            };
            emblem.SetBinding(Image.SourceProperty, UploadingViewModel.EmblemProperty);

            var spinner = new CircleImage
            {
                Source = "Resources/spinner.gif",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
            };
            spinner.SetBinding(Image.SourceProperty, UploadingViewModel.SpinnerProperty);

            var text = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            text.SetBinding(Label.TextProperty, UploadingViewModel.UploaderTextProperty);

            var img = new Image
            {
                HeightRequest = 120,
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
            };
            img.SetBinding(Image.RotationProperty, UploadingViewModel.RotateProperty);
            img.SetBinding(Image.SourceProperty, UploadingViewModel.SpinnerProperty);

            var loadingStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 20, 0, 0),
                Spacing = 60,
                Children =
                {
                    img,
                    text,
                }
            };
            loadingStack.SetBinding(StackLayout.IsVisibleProperty, UploadingViewModel.UploadingVisibilityProperty);

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 70, 10, 10),
                Children =
                {
                    emblem,
                    loadingStack,
                    ErrorStack(),
                }
            };

            return layout;
        }

        private StackLayout ErrorStack()
        {
            var text = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.LoginLabelText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center
            };
            text.SetBinding(Label.TextProperty, UploadingViewModel.ErrorTextProperty);

            var TryAgainButton = new ButtomButton("Prøv Igen", SendUploadMessage);
            var SaveButton = new ButtomButton("Gem", SendStoreMessage);
            TryAgainButton.Height = Definitions.GpsButtonHeight;
            SaveButton.Height = Definitions.GpsButtonHeight;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(50, 70, 50, 70),
                Spacing = 30,
                Children =
                {
                    text,
                    TryAgainButton,
                    SaveButton
                }
            };
            layout.SetBinding(StackLayout.IsVisibleProperty, UploadingViewModel.ErrorVisibilityProperty);

            return layout;
        }

        private void SendUploadMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Upload");
        }


        private void SendStoreMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Store");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}