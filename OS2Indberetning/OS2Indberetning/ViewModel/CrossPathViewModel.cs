using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using XLabs.Platform.Services;


namespace OS2Indberetning.ViewModel
{
    public class CrossPathViewModel : XLabs.Forms.Mvvm.ViewModel
    {
        private readonly ISecureStorage storage;
        //private readonly GalaSoft.MvvmLight.Views.INavigationService _navigationService;
        public CrossPathViewModel()
        {
            //if (navigationService == null) throw new ArgumentNullException("navigationService");
            //_navigationService = navigationService;
            //_navigationService = (NavigationService)SimpleIoc.Default.GetInstance(typeof (GalaSoft.MvvmLight.Views.INavigationService));
            storage = DependencyService.Get<ISecureStorage>();
            Subscribe();
            App.ShowLoading(true);
            //IsUserStillValid();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<CrossPathPage>(this, "Choose", (sender) =>
            {
                IsUserStillValid();
            });
        }

        private void ShowMainPage()
        {
            Definitions.Report = new DriveReport();
            App.ShowLoading(false, true);
            App.Navigation.PopToRootAsync();
            //_navigationService.NavigateTo(Locator.MainPage, null);
        }

        private void ShowLoginPage()
        {
            Definitions.Report = new DriveReport();
            App.ShowLoading(false, true);
            Navigation.PushAsync<LoginViewModel>();
        }

        private void IsUserStillValid()
        {
            try
            {
                // Get Municipality from storage and deserialize
                if (!storage.Contains(Definitions.TokenKey))
                {
                    ShowLoginPage();
                    return;
                }
                var byteArray = storage.Retrieve(Definitions.TokenKey);
                var mstring = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var userToken = JsonConvert.DeserializeObject<Token>(mstring);
                // Get Token from storage and deserialize
                if (!storage.Contains(Definitions.MunKey))
                {
                    ShowLoginPage();
                    return;
                }
                var userTokenByte = storage.Retrieve(Definitions.MunKey);
                var userTokenString = Encoding.UTF8.GetString(userTokenByte, 0, userTokenByte.Length);
                var mun = JsonConvert.DeserializeObject<Municipality>(userTokenString);

                APICaller.RefreshModel(userToken, mun).ContinueWith((result) =>
                {
                    if (result.Result == null)
                    {
                        ShowLoginPage();
                        return;
                    }

                    Definitions.User = result.Result;
                    Definitions.MunIcon = new UriImageSource { Uri = new Uri(mun.ImgUrl) };
                    Definitions.TextColor = mun.TextColor;
                    Definitions.PrimaryColor = mun.PrimaryColor;
                    Definitions.SecondaryColor = mun.SecondaryColor;
                    Definitions.MunUrl = mun.APIUrl;
                    storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Result)));

                    ShowMainPage();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception e)
            {
                App.ShowLoading(false, true);
                ShowLoginPage();
                return;
            }
        }
    }
}
