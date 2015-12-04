using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject.Modules;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning.Pages
{
    public class CrossPathPage : ContentPage
    {
        private ISecureStorage storage;
        private bool HasRunOnce;

        public CrossPathPage()
        {
            storage = DependencyService.Get<ISecureStorage>();
            this.Content = SetTempContent();
            App.ShowLoading(true);
            IsUserStillValid();
        }

        private void IsUserStillValid()
        {
            try
            {
                // Get Municipality from storage and deserialize
                var byteArray = storage.Retrieve(Definitions.TokenKey);
                var mstring = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var userToken = JsonConvert.DeserializeObject<Token>(mstring);
                // Get Token from storage and deserialize
                var userTokenByte = storage.Retrieve(Definitions.MunKey);
                var userTokenString = Encoding.UTF8.GetString(userTokenByte, 0, userTokenByte.Length);
                var mun = JsonConvert.DeserializeObject<Municipality>(userTokenString);

                APICaller.RefreshModel(userToken, mun).ContinueWith((result) =>
                {
                    if (result.Result == null)
                    {
                        App.ShowLoading(false, true);
                        Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<LoginViewModel, LoginPage>());
                        return;
                    }

                    Definitions.User = result.Result;
                    Definitions.MunIcon = new UriImageSource { Uri = new Uri(mun.ImgUrl) };
                    Definitions.TextColor = mun.TextColor;
                    Definitions.PrimaryColor = mun.PrimaryColor;
                    Definitions.SecondaryColor = mun.SecondaryColor;
                    storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Result)));
                    App.ShowLoading(false, true);
                    HasRunOnce = true;
                    Definitions.Report = new DriveReport();
                    Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<MainViewModel, MainPage>());
                    return;
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception e)
            {
                App.ShowLoading(false, true);
                Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<LoginViewModel, LoginPage>());
                return;
            }
        }

        protected override void OnAppearing()
        {
            // If Drive report was deleted, returns to mainview with clean report
            if (HasRunOnce)
            {
                Definitions.Report = new DriveReport();
                Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<MainViewModel, MainPage>());
            }
        }



        private View SetTempContent()
        {

            var layout = new StackLayout
            {
               
            };

            return layout;
        }
    }
}
