/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
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
    /// <summary>
    /// Viewmodel of the CrossPath page. Handles all view logic 
    /// </summary>
    public class CrossPathViewModel : XLabs.Forms.Mvvm.ViewModel, IDisposable
    {
        private readonly ISecureStorage _storage;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public CrossPathViewModel()
        {
            _storage = DependencyService.Get<ISecureStorage>();
            Subscribe();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<CrossPathPage>(this, "Choose", (sender) =>
            {
                IsUserStillValid();
            });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// Important this is called upon popping of the page
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<CrossPathPage>(this, "Choose");
        }

        /// <summary>
        /// Method that pops back to the viewmodel and does some cleanup and initialization
        /// </summary>
        private void ShowMainPage()
        {
            Definitions.Report = new DriveReport();
            Dispose();
            App.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that navigaties to the login page and does some cleanup and initialization
        /// </summary>
        private void ShowLoginPage()
        {
            Definitions.Report = new DriveReport();
            Dispose();
            Navigation.PushAsync<LoginViewModel>();
        }

        /// <summary>
        /// Method that decides what page to be displayed.
        /// Shows login page if the user has never logged in before or the stored userdata is invalid
        /// Shows mainpage if the stored userdata is valid
        /// </summary>
        private void IsUserStillValid()
        {
            try
            {
                // Get Municipality from storage and deserialize
                if (_storage.Retrieve(Definitions.TokenKey) == null)
                {
                    ShowLoginPage();
                    return;
                }
                var byteArray = _storage.Retrieve(Definitions.TokenKey);
                var mstring = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var userToken = JsonConvert.DeserializeObject<Token>(mstring);
                // Get Token from storage and deserialize
                if (_storage.Retrieve(Definitions.MunKey) == null)
                {
                    ShowLoginPage();
                    return;
                }
                if (_storage.Retrieve(Definitions.UserDataKey) == null)
                {
                    ShowLoginPage();
                    return;
                }
                var munByte = _storage.Retrieve(Definitions.MunKey);
                var munString = Encoding.UTF8.GetString(munByte, 0, munByte.Length);
                var mun = JsonConvert.DeserializeObject<Municipality>(munString);

                var userByte = _storage.Retrieve(Definitions.UserDataKey);
                var userString = Encoding.UTF8.GetString(userByte, 0, userByte.Length);
                var user = JsonConvert.DeserializeObject<UserInfoModel>(userString);

                APICaller.RefreshModel(userToken, mun).ContinueWith((result) =>
                {
                    if (result.Result.Error.ErrorCode == "404")
                    {
                        App.ShowMessage(result.Result.Error.ErrorMessage + "\n" +
                                        "Data kunne ikke hentes. Gemte data benyttes.");

                        Definitions.User = user;
                        Definitions.MunIcon = new UriImageSource { Uri = new Uri(mun.ImgUrl) };
                        Definitions.TextColor = mun.TextColor;
                        Definitions.PrimaryColor = mun.PrimaryColor;
                        Definitions.SecondaryColor = mun.SecondaryColor;
                        Definitions.MunUrl = mun.APIUrl;
                        ShowMainPage();
                    }
                    else if (result.Result.User == null)
                    { 
                        App.ShowMessage("Fejl: " + result.Result.Error.ErrorMessage);
                        ShowLoginPage();
                        return;
                    }

                    Definitions.User = result.Result.User;
                    Definitions.MunIcon = new UriImageSource { Uri = new Uri(mun.ImgUrl) };
                    Definitions.TextColor = mun.TextColor;
                    Definitions.PrimaryColor = mun.PrimaryColor;
                    Definitions.SecondaryColor = mun.SecondaryColor;
                    Definitions.MunUrl = mun.APIUrl;
                    _storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Result)));

                    ShowMainPage();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception e)
            {
                ShowLoginPage();
                return;
            }
        }
    }
}
