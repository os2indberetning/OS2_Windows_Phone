using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    public class CrossPathPage : ContentPage
    {
        

        public CrossPathPage()
        {
            //BindingContext = App.Locator.CrossPath;
            this.Content = SetTempContent();
        }

        
        private View SetTempContent()
        {

            var layout = new StackLayout();

            return layout;
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Send(this, "Choose");
        }
    }
}
