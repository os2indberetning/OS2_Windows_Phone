/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that makes it possible for the user to add new purposes to a _list and select a specific purpose
    /// </summary>
    public class PurposePage : ContentPage
    {
        public PurposeString Selected;

        private int _hackSpaces;
        private string _placeholder = "";

        /// <summary>
        /// Constructor that handles initialization of the page
        /// </summary>
        public PurposePage()
        {

            // HACK to center placeholder text. best method i could find
            _hackSpaces = (int)Math.Round(Definitions.ScreenWidth / 46);
            for (int i = 0; i < _hackSpaces; i++)
            {
                _placeholder = _placeholder + " ";
            }
            _placeholder = _placeholder + "Tast nyt formål";

            SetContent();
        }

        /// <summary>
        /// Method that creates the page layout and sets the page content, to that layout
        /// </summary>
        public void SetContent()
        {
            var header = new Label
            {
                Text = "Vælg Formål",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var addButton = new AddButton(SendAddMessage);
            var backButton = new BackButton(SendBackMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    addButton
                }
            };

            var list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GenericCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, PurposeViewModel.PurposeListProperty, BindingMode.TwoWay);

            list.ItemSelected += (sender, args) =>
            {
                var item = (PurposeString) args.SelectedItem;

                if (item == null) return;
                item.Selected = true;
                Selected = item;
                SendSelectedMessage();
            };

            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            var entry = new Entry
            {
                Placeholder = _placeholder,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
            };
            entry.SetBinding(Entry.TextProperty, PurposeViewModel.PurposeStringProperty);

            var addPurpose = new Button
            {
                Text = "Tilføj",
                BorderColor = Color.FromHex(Definitions.PrimaryColor),
                TextColor = Color.FromHex(Definitions.TextColor),
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                AnchorY = (double)TextAlignment.Center,
                FontSize = 20,
                
            };
            addPurpose.SetBinding(Button.CommandProperty, PurposeViewModel.AddPurposeCommand);

            var addStack = new StackLayout
            {
                Padding = new Thickness(Definitions.Padding, 0, Definitions.Padding, 0),
                Spacing = Definitions.Padding,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    entry,
                    addPurpose
                }
            };
            addStack.SetBinding(StackLayout.IsVisibleProperty, PurposeViewModel.HideFieldProperty);

            layout.Children.Add(headerstack);
            layout.Children.Add(addStack);
            layout.Children.Add(list);

            this.Content = layout;

        }

        #region Message Handlers

        /// <summary>
        /// Method that handles sending an Add message
        /// </summary>
        private void SendAddMessage()
        {
            MessagingCenter.Send<PurposePage>(this, "Add");
        }

        /// <summary>
        /// Method that handles sending an Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send(this, "Back");
        }

        /// <summary>
        /// Method that handles sending an Selected message
        /// </summary>
        private void SendSelectedMessage()
        {
            MessagingCenter.Send<PurposePage>(this, "Selected");
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override of the OnBackButtonPressed event.
        /// Calls SendBackMessage so the viewmodel handles the back event
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
