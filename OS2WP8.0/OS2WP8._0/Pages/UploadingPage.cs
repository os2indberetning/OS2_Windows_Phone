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

namespace OS2Indberetning
{
    /// <summary>
    /// Page that is displayed when trying to upload a drivereport
    /// If upload fails, the user can choose to retry or store the report
    /// </summary>
    public class UploadingPage : ContentPage
    {
        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public UploadingPage()
        {
            this.Content = this.SetContent();
            SendUploadMessage();
        }

        /// <summary>
        /// Method that creates the page content
        /// </summary>
        public View SetContent()
        {
            var emblem = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 160
            };
            emblem.SetBinding(Image.SourceProperty, UploadingViewModel.EmblemProperty);

            var spinner = new CircleImage
            {
                Source = "Resources/spinner.gif",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
            };
            spinner.SetBinding(Image.SourceProperty, UploadingViewModel.SpinnerProperty);

            var text = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.HeaderFontSize - 8,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            text.SetBinding(Label.TextProperty, UploadingViewModel.UploaderTextProperty);

            var img = new Image
            {
                HeightRequest = 120,
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
            };
            img.SetBinding(Image.RotationProperty, UploadingViewModel.RotateProperty);
            img.SetBinding(Image.SourceProperty, UploadingViewModel.SpinnerProperty);

            var loadingStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 20, 0, 0),
                Spacing = 60,
                Children =
                {
                    img,
                    text,
                }
            };
            loadingStack.SetBinding(StackLayout.IsVisibleProperty, UploadingViewModel.UploadingVisibilityProperty);

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 70, 10, 10),
                Children =
                {
                    emblem,
                    loadingStack,
                    ErrorStack(),
                }
            };

            return layout;
        }

        /// <summary>
        /// Method that creates the error stack
        /// The stacks visibility is binded to the viewmodel
        /// </summary>
        private StackLayout ErrorStack()
        {
            var text = new Label
            {
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.LoginLabelText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center
            };
            text.SetBinding(Label.TextProperty, UploadingViewModel.ErrorTextProperty);

            var TryAgainButton = new ButtomButton("Prøv Igen", SendUploadMessage);
            var SaveButton = new ButtomButton("Gem", SendStoreMessage);
            TryAgainButton.Height = Definitions.GpsButtonHeight;
            SaveButton.Height = Definitions.GpsButtonHeight;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(50, 70, 50, 70),
                Spacing = 30,
                Children =
                {
                    text,
                    TryAgainButton,
                    SaveButton
                }
            };
            layout.SetBinding(StackLayout.IsVisibleProperty, UploadingViewModel.ErrorVisibilityProperty);

            return layout;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles sending a Upload message
        /// </summary>
        private void SendUploadMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Upload");
        }

        /// <summary>
        /// Method that handles sending a Store message
        /// </summary>
        private void SendStoreMessage()
        {
            MessagingCenter.Send<UploadingPage>(this, "Store");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Makes backbutton press invalid by doing nothing and returning true
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion
    }
}