/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that is displayed when trying to couple your phone with a Municipality
    /// </summary>
    public class CouplingPage : ContentPage
    {
        private Municipality _municipality;

        private int hackSpaces;
        private string _usernamePlaceholder = "  Brugernavn";
        private string _pwPlaceholder = "  Password";

        public CouplingPage()
        {

        }

        /// <summary>
        /// Method is used as a constructor. The real constructor needs to be parameterless
        /// </summary>
        /// <param name="m">Municipality that the user is trying to couple with</param>
        public void SetMunicipality(Municipality m)
        {
            _municipality = m;
            Definitions.TextColor = _municipality.TextColor;
            Definitions.PrimaryColor = _municipality.PrimaryColor;
            Definitions.SecondaryColor = _municipality.SecondaryColor;
            Definitions.MunIcon = new UriImageSource {Uri = new Uri(m.ImgUrl)};
            Definitions.MunUrl = _municipality.APIUrl;
            this.Content = SetContent();
        }


        /// <summary>
        /// Method used to create the layout of the page
        /// </summary>
        private View SetContent()
        {

            var header = new Label
            {
                Text = "Log ind",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
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
                Children =
                {
                    backButton,
                    header,
                    filler
                }
            };
            var usernameLabel = new Label
            {
                Text = _usernamePlaceholder,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
            };
            var username = new Entry
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
            };
            username.SetBinding(Entry.TextProperty, CouplingViewModel.UsernameProperty);

            var pwLabel = new Label
            {
                Text = _pwPlaceholder,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
            };
            var pw = new Entry
            {
                
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                IsPassword = true,
            };
            pw.SetBinding(Entry.TextProperty, CouplingViewModel.PasswordProperty);

            pw.Completed += (sender, args) =>
            {
                SendCoupleMessage();
            };

            var informationText = new Label
            {
                Text = "Du skal oprette et password for at logge ind på app'en. \n\n" +
                       "Passwordet opretter du i OS2Indberetning under Personlige Indstillinger: Adgangskode til app.\n\n" +
                       "Her finder du også dit brugernavn, som er dine initialer.\n",
                       
                HorizontalOptions = LayoutOptions.Center,
                YAlign = TextAlignment.Center,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.InformationFontSize,
                
            };

            var version = new Label
            {
                Text = "version " + Definitions.VersionNumber,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                YAlign = TextAlignment.End,
            };

            var textFrame = new StackLayout
            {
                Padding = Definitions.Padding,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children =
                {
                    informationText
                }
            };

            var coupleButton = new ButtomButton("Log ind", SendCoupleMessage);
           
            var buttomStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Start,
                Padding = Definitions.Padding,
                Spacing = 2,
                HeightRequest = Definitions.ButtonHeight,

                Children = {  coupleButton }
            };

            var layout = new StackLayout
            {
                Children =
                {
                    headerstack,
                    usernameLabel,
                    username,
                    pwLabel,
                    pw,
                    buttomStack,
                    textFrame,
                    version
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            return layout;
        }

        /// <summary>
        /// Method that sends a Couple message
        /// </summary>
        private void SendCoupleMessage()
        {
            MessagingCenter.Send<CouplingPage>(this, "Couple");
        }

        /// <summary>
        /// Method that sends a Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send<CouplingPage>(this, "Back");
        }

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Calls SendBackMessage so that the logic is handles by the viewmodel
        /// </summary>
        /// <returns>true so that the event is "overlooked"</returns>
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<CouplingPage>(this, "Dispose");
        }
    }


}
