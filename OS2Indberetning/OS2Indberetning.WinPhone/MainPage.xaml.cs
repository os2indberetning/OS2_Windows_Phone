using Microsoft.Phone.Controls;

namespace OS2Indberetning.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            //SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
            SupportedOrientations = SupportedPageOrientation.Portrait; // Force Portrait

            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new OS2Indberetning.App());
        }
    }
}
