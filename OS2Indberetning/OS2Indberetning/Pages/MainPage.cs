using System;
using OS2Indberetning.Model;
using OS2Indberetning.PlatformInterfaces;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning
{
    public class MainPage : ContentPage
    {

        private double popupWidth = Definitions.ScreenWidth - Definitions.Padding;
        private readonly double yesNoButtonWidth = (Definitions.ScreenHeight - Definitions.Padding) / 2;
        private double popupHeight = Definitions.ScreenHeight / 3;

        public ListView list;
        private PopupLayout _PopUpLayout;

        public MainPage()
        {

            //var byteArray = storage.Retrieve(Definitions.UserDataKey);
            //user = JsonConvert.DeserializeObject<UserInfoModel>(Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
            
            this.Content = this.SetContent();
            
        }

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
            var filler = new Filler();

            vertButton.WidthRequest = 60;
            vertButton.HeightRequest = 60;
            filler.WidthRequest = 60;
            filler.HeightRequest = 60;

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 5,
                Children =
                {
                    filler,
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

            list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DriveReportCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            list.SetBinding(ListView.ItemsSourceProperty, MainViewModel.DriveProperty);



            list.ItemSelected += async (sender, e) =>
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
                WidthRequest = yesNoButtonWidth,
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

        private void ClosePopup()
        {
            _PopUpLayout.DismissPopup();
        }

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

            var check = new CheckboxButton(ToggleHomeProperty);

            return new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { label, check}
            };
        }


        private void StartDrive()
        {
            if (Definitions.Report.Purpose == null)
            {
                _PopUpLayout.ShowPopup(CreatePopup("Vælg venligst et formål"));
                return;
            }
            if (Definitions.Report.EmploymentId == 0)
            {
                _PopUpLayout.ShowPopup(CreatePopup("Vælg venligst en organisatorisk placering"));
                return;
            }
            if (Definitions.Report.Rate == null)
            {
                _PopUpLayout.ShowPopup(CreatePopup("Vælg venligst en takst"));
                return;
            }
            MessagingCenter.Send<MainPage>(this, "Start");
        }

        private void ToggleHomeProperty()
        {
            MessagingCenter.Send<MainPage>(this, "ToggleHome");
        }

        private void SendSelectedMessage()
        {
            MessagingCenter.Send<MainPage>(this, "Selected");
        }

        private void SendViewStoredMessage()
        {
            MessagingCenter.Send<MainPage>(this, "ViewStored");
        }

        protected override bool OnBackButtonPressed()
        {
            //if (Device.OS == TargetPlatform.WinPhone)
            //{
            //    DependencyService.Get<IPlatformMethods>().TerminateApp();
            //}
            return true;
        }

        protected override void OnAppearing()
        {
            if (list != null)
            {
                list.SelectedItem = null;
            }
            MessagingCenter.Send<MainPage>(this, "Update");
        }
    }
}