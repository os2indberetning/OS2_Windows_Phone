/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel to the Login page. Setups the list for display in the page
    /// and handles pushing to the couple page with the correct model
    /// </summary>
    public class LoginViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //private ICommand _refreshListCommand;

        private ObservableCollection<MunCellModel> _munList;
        private List<Municipality> _objectList;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public LoginViewModel()
        {
            Definitions.PrimaryColor = Definitions.DefaultPrimaryColor;
            Definitions.SecondaryColor = Definitions.DefaultSecondaryColor;
            Definitions.BackgroundColor = Definitions.DefaultBackgroundColor;
            Definitions.TextColor = Definitions.DefaultTextColor;

            MunList = new ObservableCollection<MunCellModel>();
            CallApi();
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
            MessagingCenter.Subscribe<LoginPage>(this, "Refresh", (sender) =>
            {
                HandleRefreshMessage();
            });

            MessagingCenter.Subscribe<LoginPage>(this, "Dispose", (sender) =>
            {
                Dispose();
            });
            MessagingCenter.Subscribe<CouplingPage>(this, "Dispose", (sender) =>
            {
                Dispose();
            });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<LoginPage>(this, "Refresh");
            MessagingCenter.Unsubscribe<LoginPage>(this, "Dispose");
            MessagingCenter.Unsubscribe<CouplingPage>(this, "Dispose");
        }

        /// <summary>
        /// Method that initializes the Municipality list
        /// </summary>
        private void InitList(List<Municipality> resultList)
        {
            _munList.Clear();

            foreach (var item in resultList)
            {
                var munCellModel = new MunCellModel { Name = item.Name };

                
                if (!string.IsNullOrEmpty(item.ImgUrl))
                {
                    munCellModel.ImageSource = new UriImageSource { Uri = new Uri(item.ImgUrl) };
                }
                else
                {
                    munCellModel.ImageSource = new UriImageSource();
                }


                _munList.Add(munCellModel);
            }

            MunList = _munList;
        }

        /// <summary>
        /// Method that handles the call to API service and directs the result
        /// </summary>
        private void CallApi()
        {
            APICaller.GetMunicipalityList().ContinueWith((result) =>
            {
                if (result.Status == TaskStatus.Faulted || result.Status == TaskStatus.Canceled)
                    return;

                _objectList = result.Result;
                InitList(result.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        /// <summary>
        /// Method called from the page when an item is Selected.
        /// It pushes a coupling page initialized to the Selected Municipality.
        /// Used instead of messagingcenter
        /// </summary>
        public void OnSelectedItem(MunCellModel selectedItem)
        {
            foreach (var item in _objectList)
            {
                if (item.Name == selectedItem.Name)
                {
                    App.Navigation.PushAsync(
                            (ContentPage)
                                ViewFactory.CreatePage<CouplingViewModel, CouplingPage>((v, vm) =>
                                {
                                    v.InitVm(item);
                                    vm.SetMunicipality(item);
                                }));
                    break;
                }
            }
        }

        /// <summary>
        /// Method that handles the call to API service and directs the result
        /// </summary>
        private void HandleRefreshMessage()
        {
            try
            {
                CallApi();
            }
            catch
            {
               
            }
            
        }

        #region Properties

        public const string MunListProperty = "MunList";
        public ObservableCollection<MunCellModel> MunList
        {
            get { return _munList; }
            set
            {
                _munList = value;
                OnPropertyChanged(MunListProperty);
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
