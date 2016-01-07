/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that makes it possible to add remarks to a drive report in the form of text
    /// </summary>
    public class RemarkPage : ContentPage
    {
        private Editor _editor;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public RemarkPage()
        {
            SetContent();
        }

        /// <summary>
        /// Method that creates the page content
        /// </summary>
        public void SetContent()
        {
            var header = new Label
            {
                Text = "Bemærkning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            var saveButton = new SaveButton(SendSaveMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    saveButton
                }
            };
            _editor = new Editor
            {
                HeightRequest = Definitions.ScreenHeight - Definitions.HeaderHeight - 2 * Definitions.Padding,
                WidthRequest = Definitions.ScreenWidth,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            _editor.SetBinding(Editor.TextProperty, RemarkViewModel.RemarkProperty);

            var editorStack = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = Definitions.Padding,
                Children = { _editor }
            };
            
            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                Children = { headerstack, editorStack}
            };

            this.Content = layout;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles sending a Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Back");
        }

        /// <summary>
        /// Method that handles sending a Save message
        /// </summary>
        private void SendSaveMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Save");
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

        #endregion
    }
}
