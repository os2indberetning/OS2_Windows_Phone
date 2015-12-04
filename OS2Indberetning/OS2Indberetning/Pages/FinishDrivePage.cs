using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using XLabs.Forms.Services;
using XLabs.Helpers;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Media;

namespace OS2Indberetning
{
    public class FinishDrivePage : ContentPage
    {
        public ListView list;
        private PopupLayout _PopUpLayout;

        private int popupWidth = Resolver.Resolve<IDevice>().Display.Width - 30;
        private int yesNoSpacing = 10;
        private int yesNoButtonWidth = Resolver.Resolve<IDevice>().Display.Width / 2;
        private int popupHeight = 300;

        public FinishDrivePage()
        {

            //var byteArray = storage.Retrieve(Definitions.UserDataKey);
            //user = JsonConvert.DeserializeObject<UserInfoModel>(Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
            
            this.Content = this.SetContent();
            
        }

        public View SetContent()
        {
            var header = new Label
            {
                Text = "Afslut Kørsel",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Padding = 5,
                Children =
                {
                    header,
                }
            };

            var date = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 40,
            };
            date.SetBinding(Label.TextProperty, FinishDriveViewModel.DateProperty);
            var user = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 40,
            };
            user.SetBinding(Label.TextProperty, FinishDriveViewModel.UsernameProperty);

            list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DriveFinishedCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            list.SetBinding(ListView.ItemsSourceProperty, MainViewModel.DriveProperty);

            var startButton = new ButtomButton("Indsend Kørsel", SendUploadMessage);
            var cancelButton = new ButtomButton("Annuller og Slet", OpenPopup);
            startButton.FontSize = 24;
            cancelButton.FontSize = 24;
            var width = Resolver.Resolve<IDevice>().Display.Width;
            startButton.Width = width / 2;
            cancelButton.Width = width / 2 ;
            
            var buttomStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = Definitions.Padding,
                Spacing = Definitions.Padding,
                HeightRequest = Definitions.ButtonHeight + 20,
                HorizontalOptions = LayoutOptions.Fill,
                Children = {cancelButton, startButton}
            };
            
            var layout = new StackLayout
            {
                Spacing = 2,
                Children =
                {
                    headerstack,
                    date,
                    user,
                    list,
                    CheckStack(),
                    buttomStack
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            _PopUpLayout = new PopupLayout();
            _PopUpLayout.Content = layout;
            return _PopUpLayout;
        }

        private StackLayout CreatePopup()
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Bekræft Sletning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,
            };
            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    header,
                }
            };
            var text = new Label
            {
                Text = "Kørslen vil ikke blive gemt",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
            };

            var noButton = new ButtomButton("Fortryd", ClosePopup);
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("OK", DeleteAndClosePopup);
            var yesStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = yesNoButtonWidth,
                Children = { yesButton }
            };

            var ButtonStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, 0),
                Spacing = Definitions.Padding,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    noStack,
                    yesStack
                }
            };

            var PopUp = new StackLayout
            {
                WidthRequest = popupWidth,
                HeightRequest = popupHeight,
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    headerstack,
                    text,
                    ButtonStack
                }
            };
            var topPadding = display.Height / 2 - 150;
            var PopUpBackground = new StackLayout
            {
                Padding = new Thickness(0, topPadding, 0, 0),
                WidthRequest = display.Width,
                HeightRequest = display.Height,
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.85),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    PopUp
                }
            };

            return PopUpBackground;
        }

        private void OpenPopup()
        {
            _PopUpLayout.ShowPopup(CreatePopup());
        }

        private void ClosePopup()
        {
            _PopUpLayout.DismissPopup();
        }

        private void DeleteAndClosePopup()
        {
            // Doing some cleanup
            Definitions.Report = null;
            (this.BindingContext as FinishDriveViewModel).Dispose();
            this.BindingContext = null;
            Navigation.PopToRootAsync( );
        }

        private void SendUploadMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Upload");
        }


        private StackLayout CheckStack()
        {
            var startLabel = new Label
            {
                Text = "Starter du hjemme?",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            var endLabel = new Label
            {
                Text = "Sluttede du hjemme?",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            var startCheck = new CheckboxButton(null, Definitions.Report.StartsAtHome);
            var endCheck = new CheckboxButton(null, Definitions.Report.EndsAtHome);

            var topCheck = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { startLabel, startCheck}
            };
            var buttomCheck = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { endLabel, endCheck }
            };

            return new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Spacing = 5,
                Children = { topCheck, buttomCheck }
            };
        }


        private void StartDrive()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Start");
        }

        private void ToggleHomeProperty()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "ToggleHome");
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }

        protected override void OnAppearing()
        {
            if (list != null)
            {
                list.SelectedItem = null;
            }
            base.OnAppearing();
            MessagingCenter.Send(this, "Update");
        }
    }
}