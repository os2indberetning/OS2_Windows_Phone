/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning
{
    /// <summary>
    /// Page that makes it possible to view and send or delete stored reports
    /// </summary>
    public class StoredReportsPage : ContentPage
    {
        // Height and Width definitions
        private readonly double _popupWidth = Definitions.ScreenWidth - 2 * Definitions.Padding;
        private readonly double _yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 3;
        private readonly double _popupHeight = Definitions.ScreenHeight / 3.6;
        
        // public layout items so they are reachable from the viewmodel.
        public ListView List;
        public PopupLayout PopUpLayout;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public StoredReportsPage()
        {
            this.Content = this.SetContent();
        }

        /// <summary>
        /// Method that creates the page content
        /// </summary>
        public View SetContent()
        {
            var header = new Label
            {
                Text = "Gemte Rapporter",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize - 4,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            var backButton = new BackButton(SendBackMessage);
            var filler = new Filler();
            
            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Padding = 5,
                Children =
                {
                    backButton,
                    header,
                    filler,
                }
            };

            var topText = new Label
            {
                Text = "Klik på rapporten for enten at sende eller slette den",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.LoginLabelText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,
            };

            List = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(StoredReportCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            List.SetBinding(ListView.ItemsSourceProperty, StoredReportsViewModel.ListProperty);



            List.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                var selectedItem = (StoredReportCellModel)e.SelectedItem;
                PopUpLayout.ShowPopup(CreatePopup("Send rapport fra d. " + selectedItem.report.Date + " ?"));
            };
            
            var layout = new StackLayout
            {
                Spacing = 8,
                Children =
                {
                    headerstack,
                    topText,
                    List,
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            PopUpLayout = new PopupLayout();
            PopUpLayout.Content = layout;
            return PopUpLayout;
        }

        #region Popups

        /// <summary>
        /// Method that creates the stacklayout for the popup
        /// </summary>
        /// <param name="message">Text to be displayed in the popup</param>
        /// <returns>Stacklayout of the popup</returns>
        private StackLayout CreatePopup(string message)
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Send Rapport",
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
                Text = message,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,
            };

            var sendButton = new ButtomButton("Send", SendUploadMessage);
            var cancelButton = new ButtomButton("Fortryd", ClosePopup);
            var removeButton = new ButtomButton("Slet", SendRemoveMessage);
            sendButton.WidthRequest = _yesNoButtonWidth;
            cancelButton.WidthRequest = _yesNoButtonWidth;
            removeButton.WidthRequest = _yesNoButtonWidth;
            var noStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.ButtonHeight,
                Spacing = Definitions.Padding,
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, Definitions.Padding),
                Children = { cancelButton, sendButton, removeButton}
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
                    noStack
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
        /// Method that creates the stacklayout for the message popup
        /// </summary>
        /// <param name="message">Text to be displayed in the popup</param>
        /// <returns>Stacklayout of the popup</returns>
        public StackLayout CreateMessagePopup(string message)
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Status",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
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
                Text = message,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,
            };

            var textStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, 0),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    text,
                }
            };
            
            var cancelButton = new ButtomButton("Ok", ClosePopup);

            cancelButton.WidthRequest = _yesNoButtonWidth;

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
                    cancelButton,
                }
            };

            var PopUp = new StackLayout
            {
                WidthRequest = _popupWidth,
                HeightRequest = _popupHeight,
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    headerstack,
                    textStack,
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
        /// Method that closes the popup
        /// </summary>
        public void ClosePopup()
        {
            PopUpLayout.DismissPopup();
            List.SelectedItem = null;
        }

        #endregion

        #region Message Handlers

        /// <summary>
        /// Method that handles sending a Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send(this, "Back");
        }

        /// <summary>
        /// Method that handles sending a Upload message
        /// </summary>
        private void SendUploadMessage()
        {
            MessagingCenter.Send(this, "Upload");
        }

        /// <summary>
        /// Method that handles sending a Remove message
        /// </summary>
        private void SendRemoveMessage()
        {
            MessagingCenter.Send(this, "Remove");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Calls SendBackMessage so that the logic is handles by the viewmodel
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }

        /// <summary>
        /// Method that overrides the OnAppearing event. 
        /// Removes relection from the list, so that it can be Selected again
        /// </summary>
        protected override void OnAppearing()
        {
            if (List != null)
            {
                List.SelectedItem = null;
            }
        }

        #endregion
    }
}