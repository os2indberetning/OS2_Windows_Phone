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

        private const string PurposeText = "Formål: ";
        private const string OrganizationText = "Organisatorisk placering:";
        private const string RateText = "Takst";
        private const string RemarkText = "Ekstra Bemærkning:";
        private const string NewKmText = "Antal Km:";
        private const string HomeToBorderDistanceText = "Antal Km for 4-km reglen";

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
            // View Title
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

            // Date View
            var date = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 35,
            };
            date.SetBinding(Label.TextProperty, FinishDriveViewModel.DateProperty);
            var user = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Definitions.LoginLabelText,
                HeightRequest = 35,
            };
            user.SetBinding(Label.TextProperty, FinishDriveViewModel.UsernameProperty);

            // Purpose View
            var purpose = new DriveFinishedCommonCell(SendPurposeMessage);
            purpose.Title = PurposeText;
            purpose.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.PurposeProperty);

            // Organization View
            var organization = new DriveFinishedCommonCell(SendOrganizationMessage);
            organization.Title = OrganizationText;
            organization.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.OrganizationProperty);

            // Rate View
            var rate = new DriveFinishedCommonCell(SendRateMessage);
            rate.Title = RateText;
            rate.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.RateProperty);

            // Remark View
            var remark = new DriveFinishedCommonCell(SendRemarkMessage);
            remark.Title = RemarkText;
            remark.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.RemarkProperty);

            // Total Km View
            var totalKm = new DriveFinishedCommonCell(SendSelectNewKmMessage);
            totalKm.Title = NewKmText;
            totalKm.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.NewKmProperty);

            // Home to Border Distance View 
            /*
            var homeToBorderDistance = new DriveFinishedCommonCell(SendSelectHomeToBorderDistanceMessage);
            homeToBorderDistance.Title = HomeToBorderDistanceText;
            homeToBorderDistance.SetBinding(DriveFinishedCommonCell.DetailsProperty, FinishDriveViewModel.HomeToBorderDistanceProperty);
            homeToBorderDistance.SetBinding(DriveFinishedCommonCell.IsVisibleProperty, FinishDriveViewModel.FourKmRuleCheckProperty);
            */

            // Cancel and send buttons
            var startButton = new ButtomButton("Indsend Kørsel", SendUploadMessage);
            var cancelButton = new ButtomButton("Annuller og Slet", OpenPopup);
            startButton.FontSize = 24;
            cancelButton.FontSize = 24;
            var width = Resolver.Resolve<IDevice>().Display.Width;
            startButton.Width = width / 2;
            cancelButton.Width = width / 2;

            var buttomStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                Padding = Definitions.Padding,
                Spacing = Definitions.Padding,
                HeightRequest = Definitions.ButtonHeight + 20,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { cancelButton, startButton }
            };

            // Header View Container
            var headerLayout = new StackLayout
            {
                Spacing = 2,
                Children =
                {
                    headerstack,
                    date,
                    user
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor)
            };

            // Content View Container
            var contentLayout = new StackLayout
            {
                Spacing = 2,
                Children =
                    {
                        purpose,
                        organization,
                        rate,
                        remark,
                        totalKm,
                        StartCheck(),
                        EndCheck(),
                        //FourKmRuleCheck(),
                        //homeToBorderDistance,
                        buttomStack
                    },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };
        
            // Add content view to a scrollview
            var scrollView = new ScrollView
            {
                Content = contentLayout,
            };

            // Add Header and Content View Containers to a container
            var layout = new StackLayout
            {
                Spacing = 2,
                Children =
                {
                    headerLayout,
                    scrollView
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor)
            };

            // Add layout to a PopUpLayout
            PopUpLayout = new PopupLayout();
            PopUpLayout.Content = layout;
            
            return PopUpLayout;
        }

        /// <summary>
        /// Method that creates the stacklayout for StartCheck
        /// </summary>
        /// <returns></returns>
        private StackLayout StartCheck()
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

            var startCheck = new CheckboxButton(SendStartHomeMessage, Definitions.Report.StartsAtHome);

            var startLayout = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { startLabel, startCheck }
            };

            return startLayout;
        }

        private StackLayout EndCheck()
        {
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

            var endCheck = new CheckboxButton(SendEndHomeMessage, Definitions.Report.EndsAtHome);


            var endLayout = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { endLabel, endCheck }
            };

            return endLayout;
        }

        /// <summary>
        /// Method that creates the stacklayout for the Checkboxes
        /// </summary>
        /// <returns>Stacklayout of the checkboxes</returns>
        private StackLayout FourKmRuleCheck()
        {
            var fourKmRuleLabel = new Label
            {
                Text = "Jeg bruger 4-km reglen",
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            var fourKmRuleCheck = new CheckboxButton(SendFourKmRuleMessage, Definitions.Report.FourKmRule);
            fourKmRuleCheck.SetBinding(CheckboxButton.SelectedProperty, FinishDriveViewModel.FourKmRuleCheckProperty);

            
            var fourKmRuleLayout = new StackLayout
            {
                Padding = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Children = { fourKmRuleLabel, fourKmRuleCheck }
            };
            fourKmRuleLayout.SetBinding(StackLayout.IsVisibleProperty, FinishDriveViewModel.ShowFourKmRuleProperty);

            return fourKmRuleLayout;
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
        /// Creates a view for the HomeToBorderDistance popup
        /// </summary>
        /// <returns>Stacklayout of the popup content</returns>
        public StackLayout HomeToBorderDistancePopup()
        {
            var display = Resolver.Resolve<IDevice>().Display;
            var header = new Label
            {
                Text = HomeToBorderDistanceText,
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
            entry.SetBinding(Entry.TextProperty, FinishDriveViewModel.HomeToBorderDistanceProperty);
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
            var yesButton = new ButtomButton("Gem", SendHomeToBorderDistanceMessage);
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
        /// Sends 4-km Rule message throught the MessagingCenter
        /// </summary>
        private void SendFourKmRuleMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "FourKmRule");
        }

        /// <summary>
        /// Sends Select NewKm message throught the MessagingCenter
        /// </summary>
        private void SendSelectNewKmMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "SelectNewKm");
        }

        /// <summary>
        /// Sends NewKm message throught the MessagingCenter
        /// </summary>
        private void SendNewKmMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "NewKm");
        }

        #endregion

        /// <summary>
        /// Sends Purpose message throught the MessagingCenter
        /// </summary>
        private void SendPurposeMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Purpose");
        }

        /// <summary>
        /// Sends Organization message throught the MessagingCenter
        /// </summary>
        private void SendOrganizationMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Organization");
        }

        /// <summary>
        /// Sends Rate message throught the MessagingCenter
        /// </summary>
        private void SendRateMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Rate");
        }

        /// <summary>
        /// Sends Remark message throught the MessagingCenter
        /// </summary>
        private void SendRemarkMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "Remark");
        }
        
        /// <summary>
        /// Sends Select HomeToBorderDistance message throught the MessagingCenter
        /// </summary>
        private void SendSelectHomeToBorderDistanceMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "SelectHomeToBorderDistance");
        }

        /// <summary>
        /// Sends HomeToBorderDistance message throught the MessagingCenter
        /// </summary>
        private void SendHomeToBorderDistanceMessage()
        {
            MessagingCenter.Send<FinishDrivePage>(this, "HomeToBorderDistance");
        }

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