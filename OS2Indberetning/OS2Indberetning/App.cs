﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Ninject;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Services;
using XLabs.Ioc;
using XLabs.Ioc.Ninject;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning
{
    public class App : Application
    {
        private ISecureStorage storage;

        public App()
        {
            var app = Resolver.Resolve<IXFormsApp>();
            if (app == null)
            {
                return;
            }

            app.Closing += (o, e) => Debug.WriteLine("Application Closing");
            app.Error += (o, e) => Debug.WriteLine("Application Error");
            app.Initialize += (o, e) => Debug.WriteLine("Application Initialize");
            app.Resumed += (o, e) => Debug.WriteLine("Application Resumed");
            app.Rotation += (o, e) => Debug.WriteLine("Application Rotation");
            app.Startup += (o, e) => Debug.WriteLine("Application Startup");
            app.Suspended += (o, e) => Debug.WriteLine("Application Suspended");

            //Register pages before initializing the first page
            RegisterPages();

            // For testing
            storage = DependencyService.Get<ISecureStorage>();
            //FakeModel();
            // The root page of your application
           
            //storage.Store(Definitions.TokenKey, Encoding.UTF8.GetBytes("513xc43109-2342"));
            ViewFactory.EnableCache = false;

            // Check to see if it was found
            MainPage = new NavigationPage((ContentPage)
                           ViewFactory.CreatePage<CrossPathViewModel, CrossPathPage>());
        }

        /// <summary>
        /// Get the main page
        /// </summary>
        /// <returns></returns>
        private Page GetMainPage()
        {
            return new CrossPathPage();
            // Validation test
            //if (!test)
            //{
            //    MainPage = new NavigationPage((ContentPage)
            //             ViewFactory.CreatePage<LoginViewModel, LoginPage>());
            //    return;
            //}

            //// Create the main page
            //MainPage = new NavigationPage((ContentPage)
            //    ViewFactory.CreatePage<MainViewModel, MainPage>());
        }
        
        private void RegisterPages()
        {
            ViewFactory.Register<CrossPathPage, CrossPathViewModel>();
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

        private void FakeModel()
        {
            UserInfoModel model = new UserInfoModel();
            
            model.Profile = new Profile();
            model.Rates = new List<Rate>();

            model.Rates.Add(new Rate
            {
                Description = "rate description",
                Year = "2016"
            });

            model.Rates.Add(new Rate
            {
                Description = "Cykel",
                Year = "2016"
            });

            model.Profile.Employments = new List<Employment>();
            model.Profile.Employments.Add(new Employment
            {
                EmploymentPosition = "test employment"
            });


            //Definitions.User = model;

            var token = new Token();
            var Mun = new Municipality();

            token.GuId = "077b95ab-8c38-4c39-9d5e-d8158bcd1a96";
            token.TokenString = "7460529413";

            Mun.APIUrl = "https://os2indberetningmobil.syddjurs.dk/api/";
            Mun.ImgUrl = "https://www.syddjurs.dk/sites/default/files/vaabenskjold-ikon.png";
            Mun.TextColor = "#FFFFFF";
            Mun.PrimaryColor = "#6b2d52";
            Mun.SecondaryColor = "#6583d3";

            storage.Store(Definitions.TokenKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(token)));
            storage.Store(Definitions.MunKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Mun)));
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
            // Handle when your app starts
            //var container = new SimpleContainer();
            

            //Resolver.SetResolver(container.GetResolver());
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
