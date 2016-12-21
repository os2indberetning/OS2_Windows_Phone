/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using OS2WP8._0.Model.TemplateModels;
using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page where Organization can be Selected for the drivereport
    /// </summary>
    public class OrganizationPage : ContentPage
    {
        public GenericCellModel Selected;

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
                // View title
                Text = "Vælg stilling og ansættelsessted",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            // Backbutton
            var backButton = new BackButton(SendBackMessage);
            var filler = new Filler();

            backButton.WidthRequest = backButton.WidthRequest - 40;
            filler.WidthRequest = backButton.WidthRequest;

            // Navigation Bar
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

            // Item List
            var list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GenericCell)),
                SeparatorColor = Color.FromHex("#000000"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, OrganizationViewModel.OrganizationListProperty, BindingMode.TwoWay);

            list.ItemSelected += (sender, args) =>
            {
                var item = (GenericCellModel) args.SelectedItem;

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
            MessagingCenter.Send<OrganizationPage, string>(this, "Selected", Selected.Title);
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
