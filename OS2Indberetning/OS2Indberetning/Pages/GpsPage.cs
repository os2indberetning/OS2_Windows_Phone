using System;
using System.Threading.Tasks;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning
{
    /// <summary>
    /// Page that is shown when trying the GPS signal
    /// </summary>
    public class GpsPage : ContentPage, IDisposable
    {
        // Layout that implements the option to use Popups
        private PopupLayout _popUpLayout;

        // pausebutton saved locally to make it possible to change the text on the fly
        private ToggleButton _pauseButton;

        // popup definitions
        private readonly double _popupWidth = Definitions.ScreenWidth - 2 * Definitions.Padding;
        private readonly double _yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 2;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public GpsPage()
        {
            this.Content = this.SetContent();
        }

        /// <summary>
        /// Destructor 
        /// </summary>
        public void Dispose() {}

        /// <summary>
        /// Method that creates the page content
        /// </summary>
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
                WidthRequest = _popupWidth - 80
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

            _pauseButton = new ToggleButton("Start Kørsel", "Pause Kørsel", "Genoptag Kørsel", SendToggleMessage);
            var finishButton = new ButtomButton("Afslut Kørsel", OpenPopup);
            _pauseButton.Height = Definitions.GpsButtonHeight;
            finishButton.Height = Definitions.GpsButtonHeight;

            var buttomStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(50, 0, 50, 0),
                Spacing = 30,
                Children = { _pauseButton, finishButton }
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

            _popUpLayout = new PopupLayout();
            _popUpLayout.Content = gpsStack;
            return _popUpLayout;
        }

        /// <summary>
        /// Method that handles a PauseError from the viewmodel
        /// Reinitializes the toggle button and shows error popup
        /// </summary>
        public void HandlePauseError()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _pauseButton.Text = "Genoptag kørsel";
                _popUpLayout.ShowPopup(PauseSpaceTooBigPopUp());
            });
        }

        #region Popup handling

        /// <summary>
        /// Method that creates the stacklayout for the popup
        /// </summary>
        /// <returns>Stacklayout of the popup</returns>
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
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("Ja", SendFinishMessage);
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

        /// <summary>
        /// Method that creates and opens a popup for pause error
        /// </summary>
        /// <returns>Stacklayout of the popup</returns>
        private StackLayout PauseSpaceTooBigPopUp()
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
                Text = "Du har bevæget dig fra langt væk fra punktet hvor du trykkede på pause." +
                       "Afslut eller bevæg dig inden for 200m af punktet hvor du sidst trykkede på pause.",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.PopupTextSize,
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center
            };
            

            var noButton = new ButtomButton("Annuller", ClosePopup);
            var noStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = Definitions.ButtonHeight,
                WidthRequest = _yesNoButtonWidth,
                Children = { noButton }
            };
            var yesButton = new ButtomButton("Afslut", SendFinishMessage);
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
        /// Method that opens the popup
        /// </summary>
        private void OpenPopup()
        {
            _popUpLayout.ShowPopup(CreatePopup());
        }

        /// <summary>
        /// Method that closes the popup
        /// </summary>
        private void ClosePopup()
        {
            _popUpLayout.DismissPopup();
        }

        #endregion

        #region Message Handlers

        /// <summary>
        /// Method that handles sending a toggle message
        /// </summary>
        private void SendToggleMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "Toggle");
        }

        /// <summary>
        /// Method that handles sending a finish message
        /// </summary>
        private void SendFinishMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "Finish");
        }

        /// <summary>
        /// Method that handles sending a togglefinishedhome message
        /// </summary>
        private void SendFinishedHomeMessage()
        {
            MessagingCenter.Send<GpsPage>(this, "ToggleFinishedHome");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Calls SendBackMessage so that the logic is handles by the viewmodel
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<GpsPage>(this, "Back");
            return true;
        }

        #endregion
    }
}