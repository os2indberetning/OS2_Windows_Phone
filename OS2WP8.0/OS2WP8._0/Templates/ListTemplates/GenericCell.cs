/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class GenericCell : ViewCell
    {
        public GenericCell()
        {
            var titleLabel = new Label()
            {
                FontAttributes = FontAttributes.None,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            titleLabel.SetBinding(Label.TextProperty, "Title");

            var subTitleLabel = new Label()
            {
                FontAttributes = FontAttributes.None,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListDetailTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            subTitleLabel.SetBinding(Label.TextProperty, "SubTitle");
            subTitleLabel.SetBinding(Label.IsVisibleProperty, "ShowSubTitle");

            var textLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 0, 0),
                Spacing = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    titleLabel,
                    subTitleLabel
                }
            };

            var image = new Image
            {
                Source = "Resources/done_black.png",
                HorizontalOptions = LayoutOptions.End,
                Opacity = 0.3,
                HeightRequest = 28,
                WidthRequest = 28,
            }; 
            image.SetBinding(Image.IsVisibleProperty, "Selected");

            var buttomBorder = new Label
            {
                BackgroundColor = Color.Black,
                HeightRequest = 1,
                Opacity = 0.3,
            };

            var cellLayout = new StackLayout
            {
                Spacing = 0,
                
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                
                Children = { textLayout, image, }
            };

            var wrapper = new StackLayout
            {
                Padding = new Thickness(25, 7, 25, 7),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = { cellLayout, buttomBorder, }
            };

            this.View = wrapper;
        }
    }
}
