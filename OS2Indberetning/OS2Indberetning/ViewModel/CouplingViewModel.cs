﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning.ViewModel
{
    public class CouplingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Command coupleCommand;

        private Municipality model;
        private ISecureStorage storage;
        private string token;

        public CouplingViewModel()
        {
            MessagingCenter.Subscribe<CouplingPage>(this, "Couple", (sender) => { Couple(); });
        }

        public void InitVm(Municipality m)
        {
            model = m;
            storage = DependencyService.Get<ISecureStorage>();
        }

        private void Couple()
        {
            App.ShowLoading(true);
            APICaller.Couple(model.APIUrl, token).ContinueWith((result) =>
            {
                if(result.Result == null)
                {
                    App.ShowMessage("Parring fejlede");
                    return;
                }

                var success = Couple(result.Result);
                App.ShowLoading(false, true);
                if (!success)
                {
                    App.ShowMessage("Parring fejlede");
                    return;
                }

                Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<MainViewModel, MainPage>());
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public const string TokenProperty = "Token";
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
                OnPropertyChanged(TokenProperty);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Couple(UserInfoModel user)
        {
            if (user == null)
            {
                App.ShowLoading(false, true);
                App.ShowMessage("test");
                return false;
            }
            Definitions.User = user;
            var specificToken = user.Profile.Tokens.Find(x => x.TokenString == token);
            if (specificToken == null)
            {
                App.ShowLoading(false, true);
                App.ShowMessage("test");
                return false;
            }
            storage.Store(Definitions.TokenKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(specificToken)));
            storage.Store(Definitions.MunKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
            storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));
            return true;
        }
    }
}
