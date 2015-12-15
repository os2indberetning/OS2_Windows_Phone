using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
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
                    Navigation.PushAsync<PurposeViewModel>();
                    break;
                case OrganisatoriskText:
                    Navigation.PushAsync<OrganizationViewModel>();
                    break;
                case TakstText:
                    Navigation.PushAsync<TaxViewModel>();
                    break;
                case EkstraText:
                    Navigation.PushAsync<RemarkViewModel>();
                    break;
            }
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
