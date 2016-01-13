/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Collections.Generic;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning
{
    /// <summary>
    /// Page that is shown after GPS tracking is done
    /// </summary>
    public class FinishDrivePage : ContentPage
    {
        // public layout items so they are reachable from the viewmodel.
        public ListView List;
        public PopupLayout PopUpLayout;

        // temp variable to store user inputted km number, before he pressed "ok"
        public string NewKm;

        // popup definitions
        private readonly double _popupWidth = Definitions.ScreenWidth - 2 * Definitions.Padding;
        private readonly double _yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 2;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public FinishDrivePage()
        {
            this.Content = this.SetContent();
        }

        #region View Setup
        /// <summary>
        /// Method that creates the view content
        /// </summary>
        /// <returns>the view to be displayed</returns>
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

            List = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DriveFinishedCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            List.SetBinding(ListView.ItemsSourceProperty, MainViewModel.DriveProperty);


            List.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                SendSelectedMessage();
            };

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
                    List,
                    CheckStack(),
                    buttomStack
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            PopUpLayout = new PopupLayout();
            PopUpLayout.Content = layout;
            return PopUpLayout;
        }

        /// <summary>
        /// Method that creates the stacklayout for the Checkboxes
        /// </summary>
        /// <returns>Stacklayout of the checkboxes</returns>
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

            var startCheck = new CheckboxButton(SendStartHomeMessage, Definitions.Report.StartsAtHome);
            var endCheck = new CheckboxButton(SendEndHomeMessage, Definitions.Report.EndsAtHome);

            var topCheck = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { startLabel, startCheck }
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
        #endregion

        #region Popups
        /// <summary>
        /// Creates a view for the delete report popup
        /// </summary>
        /// <returns>Stacklayout of the popup content</returns>
        private StackLayout CreateDeletePopup()
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
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("OK", SendDeleteMessage);
            var yesStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { yesButton }
            };

            var ButtonStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, Definitions.Padding),
                Spacing = Definitions.Padding,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    noStack,
                    yesStack
                }
            };

            var PopUp = new StackLayout
            {
                WidthRequest = _popupWidth,
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

        /// <summary>
        /// Creates a view for the edit km popup
        /// </summary>
        /// <returns>Stacklayout of the popup content</returns>
        public StackLayout EditKmPopup()
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Antal Km",
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
                Text = "Rediger i antal kørte km: ",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
            };

            var entry = new Entry
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                Keyboard = Keyboard.Numeric,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            entry.SetBinding(Entry.TextProperty, FinishDriveViewModel.NewKmProperty);
            entry.Focus();
            
            var noButton = new ButtomButton("Fortryd", () => PopUpLayout.DismissPopup());
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("Gem", SendNewKmMessage);
            var yesStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { yesButton }
            };

            var ButtonStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, Definitions.Padding),
                Spacing = Definitions.Padding,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    noStack,
                    yesStack
                }
            };

            var PopUp = new StackLayout
            {
                WidthRequest = _popupWidth,

                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = Definitions.Padding,
                Children =
                {
                    headerstack,
                    text,
                    entry,
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

        /// <summary>
        /// Opens the delete report popup
        /// </summary>
        private void OpenPopup()
        {
            PopUpLayout.ShowPopup(CreateDeletePopup());
        }

        /// <summary>
        /// Closes the popup
        /// </summary>
        private void ClosePopup()
        {
            PopUpLayout.DismissPopup();
        }
        #endregion

        #region Messages
        /// <summary>
        /// Sends Upload message throught the MessagingCenter
        /// </summary>
        private void SendUploadMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Upload");
        }

        /// <summary>
        /// Sends Delete message throught the MessagingCenter
        /// </summary>
        private void SendDeleteMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Delete");
        }

        /// <summary>
        /// Sends Selected message throught the MessagingCenter
        /// </summary>
        private void SendSelectedMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Selected");
        }

        /// <summary>
        /// Sends StartHome message throught the MessagingCenter
        /// </summary>
        private void SendStartHomeMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "StartHome");
        }

        /// <summary>
        /// Sends EndHome message throught the MessagingCenter
        /// </summary>
        private void SendEndHomeMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "EndHome");
        }

        /// <summary>
        /// Sends NewKm message throught the MessagingCenter
        /// </summary>
        private void SendNewKmMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "NewKm");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Opens the same popup as if the user where to delete the drive 
        /// because there is no option to return to the gps view from here
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            if (!PopUpLayout.IsPopupActive)
            {
                PopUpLayout.ShowPopup(CreateDeletePopup());
            }
            
            return true;
        }

        /// <summary>
        /// Method that overrides the OnAppearing event. 
        /// Resets the _list selection if any, and sends an Update message
        /// to the viewmodel to get the content updated.
        /// </summary>
        protected override void OnAppearing()
        {
            if (List != null)
            {
                List.SelectedItem = null;
            }
            base.OnAppearing();
            MessagingCenter.Send<FinishDrivePage>(this, "Update");
        }
        #endregion
    }
}