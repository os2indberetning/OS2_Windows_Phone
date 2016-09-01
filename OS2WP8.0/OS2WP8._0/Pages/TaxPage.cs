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
    /// Page that makes it possible to view and select a taxation catagory
    /// </summary>
    public class TaxPage : ContentPage
    {
        public GenericCellModel Selected;

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public TaxPage()
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
                Text = "Vælg Takst",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            // filler to make header is centered
            var filler = new Filler();
            // they need to be the same size.
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
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, TaxViewModel.TaxListProperty, BindingMode.TwoWay);

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
        /// Method that handles sending a Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send<TaxPage>(this, "Back");
        }

        /// <summary>
        /// Method that handles sending a Selected message
        /// </summary>
        private void SendSelectedMessage()
        {
            MessagingCenter.Send<TaxPage, string>(this, "Selected", Selected.Title);
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
