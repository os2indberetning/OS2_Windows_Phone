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
using XLabs.Helpers;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Media;

namespace OS2Indberetning
{
    public class StoredReportsPage : ContentPage
    {
        private int popupWidth = Resolver.Resolve<IDevice>().Display.Width - 30;
        private int yesNoSpacing = 10;
        private int yesNoButtonWidth = Resolver.Resolve<IDevice>().Display.Width / 2;
        private int popupHeight = 250;

        public ListView list;
        public PopupLayout _PopUpLayout;

        public StoredReportsPage()
        {
            this.Content = this.SetContent();
        }

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

            list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(StoredReportCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            list.SetBinding(ListView.ItemsSourceProperty, StoredReportsViewModel.ListProperty);



            list.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                var selectedItem = (StoredReportCellModel)e.SelectedItem;
                _PopUpLayout.ShowPopup(CreatePopup("Send rapport fra d. " + selectedItem.report.Date + " ?"));
            };
            
            var layout = new StackLayout
            {
                Spacing = 8,
                Children =
                {
                    headerstack,
                    topText,
                    list,
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
                Text = Message,
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
            sendButton.WidthRequest = yesNoButtonWidth;
            cancelButton.WidthRequest = yesNoButtonWidth;
            removeButton.WidthRequest = yesNoButtonWidth;
            var noStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Definitions.ButtonHeight,
                Spacing = yesNoSpacing,
                Children = { cancelButton, sendButton }
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
                    removeButton
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
                Padding = new Thickness(0, 0, 0, 30),
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

        public StackLayout CreateErrorPopup(string Message)
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = "Send Rapport",
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
                Text = Message,
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

            cancelButton.WidthRequest = yesNoButtonWidth;

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
                WidthRequest = popupWidth,
                HeightRequest = popupHeight,
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

        public void ClosePopup()
        {
            _PopUpLayout.DismissPopup();
            list.SelectedItem = null;
        }

        private void SendBackMessage()
        {
            MessagingCenter.Send(this, "Back");
        }

        private void SendUploadMessage()
        {
            MessagingCenter.Send(this, "Upload");
        }

        private void SendRemoveMessage()
        {
            MessagingCenter.Send(this, "Remove");
        }

        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }

        protected override void OnAppearing()
        {
            if (list != null)
            {
                list.SelectedItem = null;
            }
            //base.OnAppearing();
        }
    }
}