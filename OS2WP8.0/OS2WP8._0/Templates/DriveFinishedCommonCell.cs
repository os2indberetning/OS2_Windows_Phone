/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning.Templates
{
    public class DriveFinishedCommonCell : ContentView
    {
        public DriveFinishedCommonCell(Action callback = null)
        {
            NameLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            //NameLabel.SetBinding(Label.TextProperty, "Name");

            DescriptionLabel = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = Resolver.Resolve<IDevice>().Display.Width - 50,
            };
            //description.SetBinding(Label.TextProperty, "Description");

            var vetDetailsLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 0, 0),
                Spacing = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    NameLabel,
                    DescriptionLabel
                }
            };

            TapImage = new Image()
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
                Children = { TapImage }
            };

            var cellLayout = new StackLayout
            {
                Spacing = 5,
                Padding = new Thickness(10, 10, 15, 10),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { vetDetailsLayout, imageStack },
            };

            // add a gester reco
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command((o) =>
                {
                    if (callback != null)
                    {
                        callback.Invoke();
                    }
                })
            });

            Content = cellLayout;
        }

        public string Title
        {
            get
            {
                return NameLabel.Text;
            }
            set
            {
                if (NameLabel.Text == value)
                {
                    return;
                }

                NameLabel.Text = value;
            }
        }

        public static readonly BindableProperty DetailsProperty =
        BindableProperty.Create<DriveFinishedCommonCell, string>(w => w.Details, default(string));


        public string Details
        {
            get { return (string)GetValue(DetailsProperty); }
            set { SetValue(DetailsProperty, value); }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == DetailsProperty.PropertyName)
            {
                DescriptionLabel.Text = Details;
            }
        }

        private Label NameLabel { get; set; }
        private Label DescriptionLabel { get; set; }
        private Image TapImage { get; set; }
    }
}
