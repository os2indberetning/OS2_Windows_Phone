﻿/* 
 * Copyright (c) OS2 2016.
 *
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
    /// Page where Organization can be Selected for the drivereport
    /// </summary>
    public class OrganizationPage : ContentPage
    {
        public OrganizationString Selected;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public OrganizationPage()
        {
            SetContent();
        }

        /// <summary>
        /// Method that creates a layout for the page and sets it as the page content
        /// </summary>
        public void SetContent()
        {
            var header = new Label
            {
                Text = "Vælg Organisatorisk Placering",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize - 13,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            var filler = new Filler();

            backButton.WidthRequest = backButton.WidthRequest - 40;
            filler.WidthRequest = backButton.WidthRequest;

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

            var list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GenericCell)),
                SeparatorColor = Color.FromHex("#000000"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, OrganizationViewModel.OrganizationListProperty, BindingMode.TwoWay);

            list.ItemSelected += (sender, args) =>
            {
                var item = (OrganizationString) args.SelectedItem;

                if (item == null) return;
                item.Selected = true;
                Selected = item;
                SendSelectedMessage();
            };

            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };


            layout.Children.Add(headerstack);
            layout.Children.Add(list);
            
            this.Content = layout;

        }

        #region Message Handlers

        /// <summary>
        /// Method that handles sending a Back message to the viewmodel
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send<OrganizationPage>(this, "Back");
        }

        /// <summary>
        /// Method that handles sending a Selected message to the viewmodel, with the Selected items name
        /// </summary>
        private void SendSelectedMessage()
        {
            MessagingCenter.Send<OrganizationPage, string>(this, "Selected", Selected.Name);
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Override of the OnBackButtonPressed event
        /// </summary>
        /// <returns>Returns true, meaning nothing happens</returns>
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }

        #endregion
    }
}
