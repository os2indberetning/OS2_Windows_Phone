/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class DriveFinishedCell : ViewCell
    {
        public DriveFinishedCell()
        {
            var nameLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var description = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            description.SetBinding(Label.TextProperty, "Description");

            var vetDetailsLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 0, 0),
                Spacing = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    nameLabel,
                    description
                }
            };

            var tapImage = new Image()
            {
                Source = "Resources/right.png",
                IsVisible = true,
                Opacity = 0.4,
                HorizontalOptions = LayoutOptions.End,
            };

            var imageStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                Children = {tapImage}
            };

            var cellLayout = new StackLayout
            {
                Spacing = 5,
                Padding = new Thickness(10, 10, 15, 10),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {vetDetailsLayout, imageStack }
            };

            this.View = cellLayout;
        }
    }
}
