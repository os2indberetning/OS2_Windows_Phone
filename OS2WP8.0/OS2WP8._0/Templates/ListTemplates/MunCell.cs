/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Templates
{
    public class MunCell : ViewCell
    {
        public MunCell()
        {
            var vetProfileImage = new CircleImage
            {
                HeightRequest = 50,
                WidthRequest = 50,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            vetProfileImage.SetBinding(Image.SourceProperty, "ImageSource");

            var nameLabel = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.LoginLabelText,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var vetDetailsLayout = new StackLayout
            {
                Padding = new Thickness(15, 0, 0, 0),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { nameLabel }
            };

            var tapImage = new Image()
            {
                Source = "Resources/right.png",
                IsVisible = true,
                Opacity = 0.4,
                HorizontalOptions = LayoutOptions.End,
            };

            var cellLayout = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(20, 5, 20, 5),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { vetProfileImage, vetDetailsLayout, tapImage }
            };
            
            this.View = cellLayout;
        }
    }
}
