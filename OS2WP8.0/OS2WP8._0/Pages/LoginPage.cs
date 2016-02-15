/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that displays a _list of Municiplitys that the user can couple with
    /// </summary>
    public class LoginPage : ContentPage
    {
        private ListView _list;

        /// <summary>
        /// Constructor handles initialization of the page
        /// </summary>
        public LoginPage()
        {
            Definitions.PrimaryColor = Definitions.DefaultPrimaryColor;
            Definitions.SecondaryColor = Definitions.DefaultSecondaryColor;
            Definitions.BackgroundColor = Definitions.DefaultBackgroundColor;
            Definitions.TextColor = Definitions.DefaultTextColor;

            SetContent();
        }

        /// <summary>
        /// Method that creates the page content
        /// </summary>
        private void SetContent()
        {
            _list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(MunCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            _list.SetBinding(ListView.ItemsSourceProperty, LoginViewModel.MunListProperty);

            // Using this method instead of messagingcenter 
            // to make it easiere disposeable when popping to root after coupling is complete
            _list.ItemSelected += (sender, args) =>
            {
                if (args.SelectedItem == null) return;
                (BindingContext as LoginViewModel).OnSelectedItem((MunCellModel)args.SelectedItem);
            };

            var header = new Label
            {
                Text = "OS2Indberetning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize - 3,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            var filler = new Filler(null);
            var refreshButton = new RefreshButton(SendRefreshMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    refreshButton,
                    header,
                    filler,
                }
            };

            var layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    headerstack,
                    _list
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };
           
            this.Content = layout;
        }

        /// <summary>
        /// Method that handles sending a Refresh message
        /// </summary>
        public void SendRefreshMessage()
        {
            MessagingCenter.Send<LoginPage>(this, "Refresh");
        }

        #region Overrides

        /// <summary>
        /// Override of the BackbuttonPressed event. 
        /// </summary>
        /// <returns>true so that the event is "overlooked" and therefore nothing happens</returns>
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        /// <summary>
        /// Override of the OnAppearing event. 
        /// Resets Selected item, if any 
        /// </summary>
        protected override void OnAppearing()
        {
            if (_list != null)
            {
                _list.SelectedItem = null;
            }
        }
        #endregion
    }
}
