/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Text;
using Newtonsoft.Json;
using OS2Indberetning.Model;
using OS2Indberetning.PlatformInterfaces;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

namespace OS2Indberetning
{
    /// <summary>
    /// Page where drive specific selections are made before starting the drive.
    /// </summary>
    public class MainPage : ContentPage
    {

        private readonly double _popupWidth = Definitions.ScreenWidth - 2 * Definitions.Padding;
        private readonly double _yesNoButtonWidth = Definitions.ScreenWidth - 4 * Definitions.Padding;
        private readonly double _popupHeight = Definitions.ScreenHeight / 3;

        public ListView List;
        private PopupLayout _popUpLayout;
        private CheckboxButton _checkboxButton;
        private StackLayout _header;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public MainPage()
        {
            InitializeTheme();
            
        }

        /// <summary>
        /// Method that tries to initialization the theme
        /// </summary>
        public void InitializeTheme()
        {
            try
            {
                var storage = DependencyService.Get<ISecureStorage>();
                var userTokenByte = storage.Retrieve(Definitions.MunKey);
                var userTokenString = Encoding.UTF8.GetString(userTokenByte, 0, userTokenByte.Length);
                var mun = JsonConvert.DeserializeObject<Municipality>(userTokenString);

                Definitions.MunIcon = new UriImageSource {Uri = new Uri(mun.ImgUrl)};
                Definitions.TextColor = mun.TextColor;
                Definitions.PrimaryColor = mun.PrimaryColor;
                Definitions.SecondaryColor = mun.SecondaryColor;
            }
            catch (Exception e)
            {
                // no theme stored
            }
        }
        /// <summary>
        /// Method that creates the page content
        /// </summary>
        /// <returns>View of the content to be displayed</returns>
        public View SetContent()
        {
            var header = new Label
            {
                Text = "Ny Kørsel",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            var vertButton = new VertsButton(SendViewStoredMessage, Definitions.storedReportsCount.ToString());
            var exitButton = new LogoutButton(ShowLogoutPopup);

            vertButton.WidthRequest = 100;
            vertButton.HeightRequest = 60;
            exitButton.WidthRequest = 100;
            exitButton.HeightRequest = 60;

            _header = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = Definitions.HeaderHeight,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 5,
                Children =
                {
                    exitButton,
                    header,
                    vertButton,
                }
            };
            _header.SetBinding(StackLayout.BackgroundColorProperty, MainViewModel.PrimaryHexProperty);

            Definitions.DateToView = DateTime.Now.ToString("d/M/yyyy");
            Definitions.DateToApi = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var date = new Label
            {
                Text = Definitions.DateToView,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 40,
            };

            List = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DriveReportCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            List.SetBinding(ListView.ItemsSourceProperty, MainViewModel.DriveProperty);
            
            List.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                var selectedItem = (DriveReportCellModel)e.SelectedItem;
                SendSelectedMessage();
            };

            var startButton = new ButtomButton("Start Kørsel", StartDrive);
            
            var buttomStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                Padding = Definitions.Padding,
                HeightRequest = Definitions.ButtonHeight,

                Children = {startButton}
            };
            
            var layout = new StackLayout
            {
                Spacing = 2,
                Children =
                {
                    _header,
                    date,
                    List,
                    CheckStack(),
                    buttomStack
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };
            
            _popUpLayout = new PopupLayout();
            _popUpLayout.Content = layout;
            return _popUpLayout;
        }

        /// <summary>
        /// Method that creates a stacklayout of a popup with a specific message
        /// </summary>
        /// <param name="Message">string message of the text to be displayed in the popup</param>
        /// <returns>Stacklayout of popup</returns>
        private StackLayout CreatePopup(string Message)
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Fejl",
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
                Text = Message,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };


            var noButton = new ButtomButton("Ok", ClosePopup);
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
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
        /// Method that creates a stacklayout of a popup with a yes/no before logging out
        /// </summary>
        /// <returns>Stacklayout of popup</returns>
        private StackLayout LogoutPopup()
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Log Ud",
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
                Text = "Ved at logge ud slettes eventuelle gemte kørsler og formål",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center
            };
            var text2 = new Label
            {
                Text = "Vil du logge ud alligevel?",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center
            };


            var noButton = new ButtomButton("Nej", ClosePopup);
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("Ja", SendLogoutMessage);
            var yesStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { yesButton }
            };

            var ButtonStack = new StackLayout
            {
                BackgroundColor = Color.White, // for Android and WP
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(Definitions.Padding, 5, Definitions.Padding, Definitions.Padding),
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
                WidthRequest = _popupWidth,
                HeightRequest = _popupHeight,
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 4,
                Children =
                {
                    headerstack,
                    text,
                    text2,
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
        /// Method that handles closing the popup
        /// </summary>
        private void ClosePopup()
        {
            _popUpLayout.DismissPopup();
        }

        /// <summary>
        /// Method that creates a stacklayout with the checkhome checkbox
        /// </summary>
        /// <returns>Stacklayout with checbox and text</returns>
        private StackLayout CheckStack()
        {
            var label = new Label
            {
                Text = "Starter du hjemme?",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize + 3,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            if (_checkboxButton == null)
            {
                _checkboxButton = new CheckboxButton(ToggleHomeProperty);
            }

            return new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { label, _checkboxButton }
            };
        }
        
        /// <summary>
        /// Method that handles displaying of an alert popup with the correct
        /// message to notify the user that something is missing
        /// </summary>
        private void StartDrive()
        {
            if (Definitions.Report.Purpose == null)
            {
                _popUpLayout.ShowPopup(CreatePopup("Vælg venligst et formål"));
                return;
            }
            if (Definitions.Organization.Id == 0)
            {
                _popUpLayout.ShowPopup(CreatePopup("Vælg venligst en organisatorisk placering"));
                return;
            }
            if (Definitions.Rate.Id == 0)
            {
                _popUpLayout.ShowPopup(CreatePopup("Vælg venligst en takst"));
                return;
            }
            MessagingCenter.Send<MainPage>(this, "Start");
        }

        /// <summary>
        /// Method that handles displaying of an alert popup for logout
        /// </summary>
        private void ShowLogoutPopup()
        {
            if(_popUpLayout.IsPopupActive)
                _popUpLayout.DismissPopup();

            _popUpLayout.ShowPopup(LogoutPopup());
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles sending an ToggleHome message
        /// </summary>
        private void ToggleHomeProperty()
        {
            MessagingCenter.Send<MainPage>(this, "ToggleHome");
        }

        /// <summary>
        /// Method that handles sending an Selected message
        /// </summary>
        private void SendSelectedMessage()
        {
            MessagingCenter.Send<MainPage>(this, "Selected");
        }

        /// <summary>
        /// Method that handles sending an ViewStored message
        /// </summary>
        private void SendViewStoredMessage()
        {
            MessagingCenter.Send<MainPage>(this, "ViewStored");
        }

        /// <summary>
        /// Method that handles sending an Refresh message
        /// </summary>
        private void SendLogoutMessage()
        {
            MessagingCenter.Send<MainPage>(this, "Logout");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override of the OnBackButtonPressed event
        /// </summary>
        /// <returns>Returns true, meaning nothing happens</returns>
        protected override bool OnBackButtonPressed()
        {
            DependencyService.Get<IPlatformMethods>().TerminateApp();
            return true;
        }

        /// <summary>
        /// Override of the OnAppearing event.
        /// If an item is Selected in the _list it is reset.
        /// Sets the content of the page here, because it might not have been
        /// properly initialized with colors if the user wasnt coupled
        /// If its the first time its called, it will send a ShowCross message to the viewmodel.
        /// Also sends an Update message to the viewmodel
        /// </summary>
        protected override void OnAppearing()
        {
            if (List != null)
            {
                List.SelectedItem = null;
            }
            MessagingCenter.Send<MainPage>(this, "Update");

            if (this.Content == null || Definitions.RefreshMainView)
            {
                this.Content = null;
                Definitions.RefreshMainView = false;
                this.Content = SetContent();
            }
        }

        #endregion
    }
}