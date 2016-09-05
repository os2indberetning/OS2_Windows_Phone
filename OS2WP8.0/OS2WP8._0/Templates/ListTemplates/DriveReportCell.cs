/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning.Templates
{
    public class DriveReportCell : ViewCell
    {
        public DriveReportCell()
        {
            var nameLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var description = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListDetailTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = Resolver.Resolve<IDevice>().Display.Width - 50,
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
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {tapImage},
            };

            var cellLayout = new StackLayout
            {
                Spacing = 5,
                Padding = new Thickness(10, 12, 15, 12),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {vetDetailsLayout, imageStack }
            };

            this.View = cellLayout;
        }
    }
}
