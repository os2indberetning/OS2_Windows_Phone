/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;

using OS2Indberetning.Model;


namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel that handles the view logic of FinishDrivePage
    /// </summary>
    public class FinishDriveViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<DriveReportCellModel> _driveReport;

        private bool _startHomeCheck;
        private bool _endHomeCheck;
        private string _date;
        private string _username;
        private string _newKm;
        
        private const string PurposeText = "Formål: ";
        private const string OrganisatoriskText = "Organisatorisk placering:";
        private const string TakstText = "Takst";
        private const string EkstraText = "Ekstra Bemærkning:";
        private const string KilometerText = "Antal Km:";

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public FinishDriveViewModel()
        {
            _driveReport = new ObservableCollection<DriveReportCellModel>();
            NewKm = Definitions.Report.Route.TotalDistance.ToString();

            InitializeCollection();

            Username = Definitions.User.Profile.FirstName + " " + Definitions.User.Profile.LastName;
            Date = "Dato: " + Definitions.DateToView;
            StartHomeCheck = Definitions.Report.StartsAtHome;
            EndHomeCheck = Definitions.Report.EndsAtHome;
            Subscribe();
        }

        /// <summary>
        /// Destructor 
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
            _driveReport = null;
        }

        /// <summary>
        /// Method that subscribes to nessecary calls
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Upload", (sender) => UploadHandler());
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Delete", HandleDeleteMessage);
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Selected", HandleSelectedMessage);
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Update", (sender) => { HandleUpdateMessage();});
            MessagingCenter.Subscribe<FinishDrivePage>(this, "EndHome",(sender) => { StartHomeCheck = !StartHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "StartHome", (sender) => { EndHomeCheck = !EndHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "NewKm", HandleNewKmMessage);
        }

        /// <summary>
        /// Method that handles all unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Upload");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Delete");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Selected");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Update");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "EndHome");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "StartHome");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "NewKm");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            if (String.IsNullOrEmpty(Definitions.Report.ManualEntryRemark))
            {
                Definitions.Report.ManualEntryRemark = "Ingen kommentar indtastet";
            }
            _driveReport.Clear();
            DriveReportList.Clear();

            _driveReport.Add(new DriveReportCellModel
            {
                Name = PurposeText,
                Description = Definitions.Report.Purpose,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = OrganisatoriskText,
                Description = Definitions.User.Profile.Employments.FirstOrDefault(x => x.Id == Definitions.Report.EmploymentId).EmploymentPosition,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = TakstText,
                Description = Definitions.Rate.Description,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = EkstraText,
                Description = Definitions.Report.ManualEntryRemark,
            });
            _driveReport.Add(new DriveReportCellModel
            {
                Name = KilometerText,
                Description = Definitions.Report.Route.TotalDistance.ToString(),
            });

            DriveReportList = _driveReport;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles a Selected message from the page
        /// </summary>
        private void HandleSelectedMessage(FinishDrivePage sender)
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
                case KilometerText:
                    sender.PopUpLayout.ShowPopup(sender.EditKmPopup());
                    sender.List.SelectedItem = null;
                    break;
            }
        }

        /// <summary>
        /// Method that handles a Update message from the page
        /// </summary>
        private void HandleUpdateMessage()
        {
            InitializeCollection();
        }

        /// <summary>
        /// Method that handles a NewKm message from the page
        /// </summary>
        private void HandleNewKmMessage(FinishDrivePage sender)
        {
            try
            {
                Definitions.Report.Route.TotalDistance = Convert.ToDouble(_newKm);
                // When the user inputs new KM the route needs to be cleared
                Definitions.Report.Route.GPSCoordinates.Clear();
            }
            catch (Exception e)
            {
                // ONLY happens if user somehow writes letters with numeric keyboard?  
                // Can happen in a simulator
            }
            InitializeCollection();
            sender.PopUpLayout.DismissPopup();
        }

        /// <summary>
        /// Method that handles a Back message from the page
        /// Calls dispose, sets report route to null and navigates to mainpage
        /// </summary>
        private void HandleDeleteMessage(object sender)
        {
            Definitions.Report.Route = null;
            Definitions.Purpose = null;
            Dispose();
            App.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that handles a Upload message from the page
        /// Calls dispose and navigates to uploadingpage
        /// </summary>
        private void UploadHandler()
        {
            Dispose();
            Navigation.PushAsync<UploadingViewModel>();
        }

        #endregion

        #region properties
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

        public const string StartHomeCheckProperty = "StartHomeCheck";
        public bool StartHomeCheck
        {
            get
            {
                return _startHomeCheck;
            }
            set
            {
                _startHomeCheck = value;
                Definitions.Report.StartsAtHome = value;
                OnPropertyChanged(StartHomeCheckProperty);
            }
        }

        public const string EndHomeCheckProperty = "EndHomeCheck";
        public bool EndHomeCheck
        {
            get
            {
                return _endHomeCheck;
            }
            set
            {
                _endHomeCheck = value;
                Definitions.Report.EndsAtHome = value;
                OnPropertyChanged(EndHomeCheckProperty);
            }
        }

        public const string NewKmProperty = "NewKm";
        public string NewKm
        {
            get { return _newKm; }
            set
            {
                _newKm = value;
                OnPropertyChanged(NewKmProperty);
            }
        }

        public const string DateProperty = "Date";
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(DateProperty);
            }
        }

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

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
