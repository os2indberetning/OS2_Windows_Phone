/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning
{
    public class App : Application
    {
        // Navigation used when viewmodels need to pop to root.
        // Doing so without this caused problems.
        public static INavigation Navigation;

        public App()
        {
            var app = Resolver.Resolve<IXFormsApp>();
            if (app == null)
            {
                return;
            }

            // Sets the count of stored reports in the definitions file.
            ReportListHandler.GetCount();

            // Sets the height and width scale so it fits with xamarin 
            SetScreenHeightAndWidth();

            //Register pages before initializing the first page
            RegisterPages();
           
            ViewFactory.EnableCache = false;

            // The root page of your application
            this.MainPage = GetMainPage();
        }

        /// <summary>
        /// Get the main page and sets the global navigation to its navigation
        /// </summary>
        /// <returns>returns the main contentpage</returns>
        private Page GetMainPage()
        {
            var test = new NavigationPage((ContentPage) ViewFactory.CreatePage<MainViewModel, MainPage>());
            Navigation = test.Navigation;
            return test;
        }
        
        private void SetScreenHeightAndWidth()
        {
            Definitions.Padding = Definitions.ScreenWidth/32;
        }


        private void RegisterPages()
        {
            ViewFactory.Register<CouplingPage, CouplingViewModel>();
            ViewFactory.Register<LoginPage, LoginViewModel>();
            ViewFactory.Register<MainPage, MainViewModel>();
            ViewFactory.Register<PurposePage, PurposeViewModel>();
            ViewFactory.Register<TaxPage, TaxViewModel>();
            ViewFactory.Register<OrganizationPage, OrganizationViewModel>();
            ViewFactory.Register<RemarkPage, RemarkViewModel>();
            ViewFactory.Register<GpsPage, GpsViewModel>();
            ViewFactory.Register<FinishDrivePage, FinishDriveViewModel>();
            ViewFactory.Register<UploadingPage, UploadingViewModel>();
            ViewFactory.Register<StoredReportsPage, StoredReportsViewModel>();
        }

        public static void ShowLoading(bool isRunning, bool isCancel = false)
        {
           
            if (isRunning == true)
            {
                if (isCancel == true)
                {
                    UserDialogs.Instance.Loading(Definitions.LoadingMessage, new Action(async () =>
                    {
                        if (Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                        {
                            await Application.Current.MainPage.Navigation.PopModalAsync();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Navigation: Can't pop modal without any modals pushed");
                        }
                        UserDialogs.Instance.Loading().Hide();
                    }));
                }
                else
                {
                    UserDialogs.Instance.Loading(Definitions.LoadingMessage);
                }
            }
            else
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }

        public static void ShowMessage(string message, bool isCancel = false)
        {            
            UserDialogs.Instance.Alert(message);
        }

        protected override void OnStart()
        {
            MessagingCenter.Send<App>(this, "Check");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            MessagingCenter.Send<App>(this, "Check");
            if (Definitions.GpsIsActive)
            {
                MessagingCenter.Send(this, "Appeared");
            }
        }
    }
}
