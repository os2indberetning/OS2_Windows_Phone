using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class StoredReportCell : ViewCell
    {
        public StoredReportCell()
        {
            var dateLabel = new Label
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            dateLabel.SetBinding(Label.TextProperty, "Date");

            var distanceLabel = new Label
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            distanceLabel.SetBinding(Label.TextProperty, "Distance");

            var purposeLabel = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            purposeLabel.SetBinding(Label.TextProperty, "Purpose");

            var taxeLabel = new Label()
            {
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.DriveFinishedTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            taxeLabel.SetBinding(Label.TextProperty, "Taxe");
            
            // Buttom border
            var buttomBorder = new Label
            {
                BackgroundColor = Color.Black,
                HeightRequest = 1,
                Opacity = 0.3,
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(20, 10, 10, 10),
                Spacing = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    dateLabel,
                    distanceLabel,
                    purposeLabel,
                    taxeLabel,
                    buttomBorder
                }
            };

            this.View = layout;
        }
    }
}
