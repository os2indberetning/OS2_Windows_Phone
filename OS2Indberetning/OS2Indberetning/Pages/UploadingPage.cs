using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using OS2Indberetning.PlatformInterfaces;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XLabs.Forms.Controls;
using XLabs.Forms.Mvvm;
using XLabs.Helpers;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Media;

namespace OS2Indberetning
{
    public class UploadingPage : ContentPage
    {
        public UploadingPage()
        {
            //var byteArray = storage.Retrieve(Definitions.UserDataKey);
            //user = JsonConvert.DeserializeObject<UserInfoModel>(Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
            
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

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 60,
                Padding = new Thickness(10, 70, 10, 50),
                Children =
                {
                    emblem,
                    img,
                    text,
                }
            };

            return layout;
        }

        private void SendUploadedMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Uploaded");
        }

        private void SendFailedMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Failed");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}