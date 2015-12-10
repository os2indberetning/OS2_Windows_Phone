﻿using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning
{
    public class GpsPage : ContentPage
    {

        private PopupLayout _PopUpLayout;

        private double popupWidth = Definitions.ScreenWidth - 2 * Definitions.Padding;
        private readonly double yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 2;
        private double popupHeight = Definitions.ScreenHeight / 2.5;
        
        public GpsPage()
        {
            this.Content = this.SetContent();
        }

        public View SetContent()
        {
            var gpsStatus = new Label
            {

                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.AccuracyTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            gpsStatus.SetBinding(Label.TextProperty, GpsViewModel.AccuracyProperty);

            var gpsDriven = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.DrivenTextSize,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,
                WidthRequest = popupWidth - 80
            };
            gpsDriven.SetBinding(Label.TextProperty, GpsViewModel.DrivenProperty);

            var gpsUpdate = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.LastupdateTextSize,
                FontAttributes = FontAttributes.Italic,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            gpsUpdate.SetBinding(Label.TextProperty, GpsViewModel.LastUpdateProperty);

            var gpsTextStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 100, 0, 100),
                Children =
                {
                    gpsStatus,
                    gpsDriven,
                    gpsUpdate,
                }
            };

            var pauseButton = new ToggleButton("Start Kørsel", "Pause Kørsel", "Genoptag Kørsel", SendToggleMessage);
            var finishButton = new ButtomButton("Afslut Kørsel", OpenPopup);
            pauseButton.Height = Definitions.GpsButtonHeight;
            finishButton.Height = Definitions.GpsButtonHeight;

            var buttomStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(50, 0, 50, 0),
                Spacing = 30,
                Children = { pauseButton, finishButton }
            };

            var gpsStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 50, 10, 50),
                Children =
                {
                    gpsTextStack,
                    buttomStack
                }
            };

            _PopUpLayout = new PopupLayout();
            _PopUpLayout.Content = gpsStack;
            return _PopUpLayout;
        }

        private StackLayout CreatePopup()
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Afslut Kørsel?",
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
                Text = "Er du sikker på at du vil afslutte kørslen?",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
            };

            var checkboxtext = new Label
            {
                Text = "Kørsel slutter hjemme",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
            };

            var checkbox = new CheckboxButton(SendFinishedHomeMessage);

            var checkStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 20,
                Children =
                {
                    checkboxtext,
                    checkbox
                }
            };

            var noButton = new ButtomButton("Nej", ClosePopup);
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("Ja", SendFinishMessage);
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
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, Definitions.Padding),
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
                //HeightRequest = popupHeight,
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = Definitions.Padding,
                Children =
                {
                    headerstack,
                    text,
                    checkStack,
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
        
        private void SendToggleMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "Toggle");
        }

        private void SendFinishMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "Finish");
        }

        private void SendFinishedHomeMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "ToggleFinishedHome");
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<GpsPage>(this, "Back");
            return true;
        }
    }
}