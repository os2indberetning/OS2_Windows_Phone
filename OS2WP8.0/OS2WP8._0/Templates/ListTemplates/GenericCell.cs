using Xamarin.Forms;

namespace OS2Indberetning.Templates
{
    public class GenericCell : ViewCell
    {
        public GenericCell()
        {
            var nameLabel = new Label()
            {
                FontAttributes = FontAttributes.None,
                FontFamily = Definitions.FontFamily,
                FontSize = Definitions.MainListTextSize,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                YAlign = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");

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
                
                Children = {nameLabel, image, }
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
