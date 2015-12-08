using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    public class CrossPathPage : ContentPage
    {
        

        public CrossPathPage()
        {
            this.Content = SetTempContent();
        }

        
        private View SetTempContent()
        {

            var layout = new StackLayout();

            return layout;
        }

        private void SendShowMainMessage()
        {
            MessagingCenter.Send(this, "ShowMain");
        }

        protected override void OnAppearing()
        {
            if (Definitions.HasAppeared)
            {
                SendShowMainMessage();
            }
        }
    }
}
