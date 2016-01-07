/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that is displayed temporarily while deciding if the user needs to couple the phone 
    /// or the mainpage can be displayed
    /// </summary>
    public class CrossPathPage : ContentPage
    {
        /// <summary>
        /// Constructor that initializes the page
        /// </summary>
        public CrossPathPage()
        {
            this.Content = SetTempContent();
        }
        
        /// <summary>
        /// Method that created a layout
        /// </summary>
        private View SetTempContent()
        {
            var label = new Label
            {
                Text = "Loading"
            };
            var layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = { label}
            };

            return layout;
        }

        /// <summary>
        /// Override method of OnAppearing. Used to call the viewmodel at the correct time.
        /// If it was done in the constructor it could lead to the Navigation being null in the viewmodel.
        /// </summary>
        protected override void OnAppearing()
        {
            MessagingCenter.Send(this, "Choose");
        }
    }
}
