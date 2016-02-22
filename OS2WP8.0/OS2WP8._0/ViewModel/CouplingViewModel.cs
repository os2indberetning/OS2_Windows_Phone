/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.ComponentModel;
using System.Linq;
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
    /// Viewmodel of the Coupling page. Handles all view logic
    /// </summary>
    public class CouplingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Municipality _model;
        private ISecureStorage _storage;
        private string _username;
        private string _pw;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public CouplingViewModel()
        {
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
            MessagingCenter.Subscribe<CouplingPage>(this, "Couple", (sender) => { HandleCoupleMessage(); });
            MessagingCenter.Subscribe<CouplingPage>(this, "Back", (sender) =>
            {
                Dispose();
                App.Navigation.PopAsync();
            });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// Important this is called upon popping of the page
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<CouplingPage>(this, "Couple");
            MessagingCenter.Unsubscribe<CouplingPage>(this, "Back");
        }

        /// <summary>
        /// Method is used a a constructor, because the constructor needs to be parameterless
        /// </summary>
        public void InitVm(Municipality m)
        {
            _model = m;
            _storage = DependencyService.Get<ISecureStorage>();
        }

        /// <summary>
        /// Method that handles saving the user information if everything is OK
        /// </summary>
        /// <param name="user">UserInfoModel that is saved</param>
        /// <returns>True on success, false on failure</returns>
        private bool Couple(UserInfoModel user)
        {
            if (user == null)
            {
                App.ShowLoading(false, true);
                return false;
            }
            Definitions.User = user;
            //if (specificToken == null)
            //{
            //    App.ShowLoading(false, true);
            //    return false;
            //}
            _storage.Store(Definitions.AuthKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user.Profile.Authorization)));
            _storage.Store(Definitions.MunKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_model)));
            _storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));
            App.ShowLoading(false, true);
            return true;
        }

        #region Message Handlers
        /// <summary>
        /// Method that handles the couple message from the page
        /// </summary>
        private void HandleCoupleMessage()
        {
            App.ShowLoading(true);
            APICaller.Couple(_model.APIUrl, _username, _pw).ContinueWith((result) =>
            {
                if (result.Result.User == null)
                {
                    App.ShowMessage("Login fejlede\n" + "Fejl besked: " + result.Result.Error.Message);
                    return;
                }

                var success = Couple(result.Result.User);
                if (!success)
                {
                    App.ShowMessage("Login fejlede\n" + "Fejl besked: Coupling Error");
                    return;
                }

                Definitions.RefreshMainView = true;
                Dispose();
                App.Navigation.PopToRootAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Properties
        public const string UsernameProperty = "Username";
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(UsernameProperty);
            }
        }

        public const string PasswordProperty = "Pw";
        public string Pw
        {
            get
            {
                return _pw;
            }
            set
            {
                _pw = value;
                OnPropertyChanged(PasswordProperty);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
