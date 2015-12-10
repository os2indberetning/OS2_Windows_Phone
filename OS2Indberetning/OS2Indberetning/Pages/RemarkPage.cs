using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning.Pages
{
    public class RemarkPage : ContentPage
    {
        private Editor editor;
        public RemarkPage()
        {
            SetContent();
        }

        public void SetContent()
        {
            var header = new Label
            {
                Text = "Bemærkning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            var saveButton = new SaveButton(SendSaveMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    saveButton
                }
            };
            editor = new Editor
            {
                HeightRequest = Definitions.ScreenHeight,
                WidthRequest = Definitions.ScreenWidth,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            editor.SetBinding(Editor.TextProperty, RemarkViewModel.RemarkProperty);

            var editorStack = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = Definitions.Padding,
                Children = { editor }
            };
            
            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
                Children = { headerstack, editorStack}
            };

            this.Content = layout;
        }

        private void SendBackMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Back");
        }
        private void SendSaveMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Save");
        }

        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }
       
    }
}
