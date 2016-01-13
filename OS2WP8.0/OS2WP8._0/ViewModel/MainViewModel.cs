/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms;
using OS2Indberetning.Model;
using XLabs.Platform.Services;


namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Main page. Handles all view logic
    /// </summary>
    public class MainViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<DriveReportCellModel> _driveReport;
        private bool _homeCheck = false;

        private const string PurposeText = "Formål: ";
        private const string OrganisatoriskText = "Organisatorisk placering:";
        private const string TakstText = "Takst";
        private const string EkstraText = "Ekstra Bemærkning:";

        private ISecureStorage _storage;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public MainViewModel()
        {
            if (Definitions.Report == null)
            {
                Definitions.Report = new DriveReport();
            }
            
            _driveReport = new ObservableCollection<DriveReportCellModel>();
            _storage = DependencyService.Get<ISecureStorage>();
            Subscribe();
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<MainPage>(this, "Update", (sender) =>
            {
                InitializeCollection();
            });

            MessagingCenter.Subscribe<MainPage>(this, "Start", (sender) =>
            {
                Navigation.PushAsync<GpsViewModel>();
            });

            MessagingCenter.Subscribe<MainPage>(this, "ToggleHome", (sender) =>
            {
                HomeCheck = !HomeCheck;
                Definitions.StartAtHome = HomeCheck;
            });

            MessagingCenter.Subscribe<MainPage>(this, "Selected", HandleSelectedMessage);

            MessagingCenter.Subscribe<MainPage>(this, "ViewStored", (sender) =>
            {
                Navigation.PushAsync<StoredReportsViewModel>();
            });

            MessagingCenter.Subscribe<MainPage>(this, "ShowCross", (sender) =>
            {
                Navigation.PushAsync<CrossPathViewModel>();
            });

            MessagingCenter.Subscribe<MainPage>(this, "Refresh", (sender) => { HandleRefreshMessage(); });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// Since this page is only created once, we dont unsubscribe
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<MainPage>(this, "Update");

            MessagingCenter.Unsubscribe<MainPage>(this, "Start");

            MessagingCenter.Unsubscribe<MainPage>(this, "ToggleHome");

            MessagingCenter.Unsubscribe<MainPage>(this, "ViewStored");

            MessagingCenter.Unsubscribe<MainPage>(this, "ShowCross");

            MessagingCenter.Unsubscribe<MainPage>(this, "Refresh");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            _driveReport.Clear();
            DriveReportList.Clear();

            string remarkText;
            if (string.IsNullOrEmpty(Definitions.Report.ManualEntryRemark))
            {
                remarkText = "Indtast Bemærkning";
            }
            else
            {
                remarkText = Definitions.Report.ManualEntryRemark;
            }

            _driveReport.Add(new DriveReportCellModel
            {
                Name = PurposeText,
                Description = Definitions.Purpose,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = OrganisatoriskText,
                Description = Definitions.Organization.EmploymentPosition,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = TakstText,
                Description = Definitions.Taxe.Description,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = EkstraText,
                Description = remarkText,
            });

            DriveReportList = _driveReport;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles the Selected message
        /// </summary>
        private void HandleSelectedMessage(MainPage sender)
        {
            DriveReportCellModel item = sender.List.SelectedItem as DriveReportCellModel;

            switch (item.Name)
            {
                case PurposeText:
                    Navigation.PushModalAsync<PurposeViewModel>();
                      break;
                case OrganisatoriskText:
                    Navigation.PushModalAsync<OrganizationViewModel>();
                    break;
                case TakstText:
                    Navigation.PushModalAsync<TaxViewModel>();
                    break;
                case EkstraText:
                    Navigation.PushModalAsync<RemarkViewModel>();
                    break;
            }
        }

        /// <summary>
        /// Method that handles the Refresh message
        /// </summary>
        private void HandleRefreshMessage()
        {
            var byteArray = _storage.Retrieve(Definitions.TokenKey);
            var mstring = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            var userToken = JsonConvert.DeserializeObject<Token>(mstring);
            // Get Token from storage and deserialize
            if (_storage.Retrieve(Definitions.MunKey) == null)
            {
                // HMM
                return;
            }
            if (_storage.Retrieve(Definitions.UserDataKey) == null)
            {
                // HMmm
                return;
            }
            var munByte = _storage.Retrieve(Definitions.MunKey);
            var munString = Encoding.UTF8.GetString(munByte, 0, munByte.Length);
            var mun = JsonConvert.DeserializeObject<Municipality>(munString);
            APICaller.RefreshModel(userToken, mun).ContinueWith((result) =>
            {
                if (result.Result.Error.ErrorCode == "404")
                {
                    App.ShowMessage(result.Result.Error.ErrorMessage + "\nKunne Ikke opdatere.");
                    return;
                }
                else if (result.Result.User == null)
                {
                    App.ShowMessage("Fejl: " + result.Result.Error.ErrorMessage);
                    return;
                }

                Definitions.User = result.Result.User;
                Definitions.MunIcon = new UriImageSource { Uri = new Uri(mun.ImgUrl) };
                Definitions.TextColor = mun.TextColor;
                Definitions.PrimaryColor = mun.PrimaryColor;
                Definitions.SecondaryColor = mun.SecondaryColor;
                Definitions.MunUrl = mun.APIUrl;
                _storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Result)));

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Properties

        public const string DriveProperty = "DriveReportList";
        public ObservableCollection<DriveReportCellModel> DriveReportList
        {
            get { return _driveReport; }
            set
            {
                _driveReport = value;
                OnPropertyChanged(DriveProperty);
            }
        }

        public const string HomeCheckProperty = "HomeCheck";
        public bool HomeCheck
        {
            get
            {
                return _homeCheck;
            }
            set
            {
                _homeCheck = value;
                Definitions.Report.StartsAtHome = value;
                OnPropertyChanged(HomeCheckProperty);
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
