using System;
using System.Text;
using Newtonsoft.Json;
using OS2Indberetning.Model;
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

        private readonly double _popupWidth = Definitions.ScreenWidth - Definitions.Padding;
        private readonly double _yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 2;
        private readonly double _popupHeight = Definitions.ScreenHeight / 3;

        public ListView List;
        private PopupLayout _popUpLayout;
        private CheckboxButton _checkboxButton;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public MainPage()
        {
            InitializeTheme();
            //this.Content = this.SetContent();
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
            var vertButton = new VertsButton(SendViewStoredMessage);
            var refreshButton = new RefreshButton(SendRefreshMessage);

            vertButton.WidthRequest = 60;
            vertButton.HeightRequest = 60;
            refreshButton.WidthRequest = 60;
            refreshButton.HeightRequest = 60;

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //Padding = 5,
                Children =
                {
                    refreshButton,
                    header,
                    vertButton,
                }
            };
            Definitions.Date = DateTime.Now.ToString("d/M/yyyy");
            var date = new Label
            {
                Text = Definitions.Date,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 40,
            };
            Definitions.Date = DateTime.Now.ToString("d/M/yyyy");

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
                    headerstack,
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
            if (Definitions.Report.EmploymentId == 0)
            {
                _popUpLayout.ShowPopup(CreatePopup("Vælg venligst en organisatorisk placering"));
                return;
            }
            if (Definitions.Report.Rate == null)
            {
                _popUpLayout.ShowPopup(CreatePopup("Vælg venligst en takst"));
                return;
            }
            MessagingCenter.Send<MainPage>(this, "Start");
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
        private void SendRefreshMessage()
        {
            MessagingCenter.Send<MainPage>(this, "Refresh");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override of the OnBackButtonPressed event
        /// </summary>
        /// <returns>Returns true, meaning nothing happens</returns>
        protected override bool OnBackButtonPressed()
        {
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
            
            if (!Definitions.HasAppeared)
            {
                
                Definitions.HasAppeared = true;
                MessagingCenter.Send<MainPage>(this, "ShowCross");
            }
            MessagingCenter.Send<MainPage>(this, "Update");
            this.Content = SetContent();
        }

        #endregion
    }
}