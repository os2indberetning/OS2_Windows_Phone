using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

using OS2Indberetning.Model;


namespace OS2Indberetning.ViewModel
{
    public class MainViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<DriveReportCellModel> driveReport;
        private bool homeCheck = false;

        private const string purposeText = "Formål: ";
        private const string organisatoriskText = "Organisatorisk placering:";
        private const string takstText = "Takst";
        private const string ekstraText = "Ekstra Bemærkning:";

        public MainViewModel()
        {
            if (Definitions.Report == null)
            {
                Definitions.Report = new DriveReport();
            }
            Definitions.HasAppeared = true;
            driveReport = new ObservableCollection<DriveReportCellModel>();
            Subscribe();
        }

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

            MessagingCenter.Subscribe<MainPage>(this, "Selected", (sender) =>
            {
                DriveReportCellModel item = sender.list.SelectedItem as DriveReportCellModel;
                PushPageBasedOnSelectedItem(item);
            });
            MessagingCenter.Subscribe<MainPage>(this, "ViewStored", (sender) =>
            {
                Navigation.PushAsync<StoredReportsViewModel>();
            });
        }

        // Since this page is only created once, we dont unsubscribe
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<MainPage>(this, "Update");

            MessagingCenter.Unsubscribe<MainPage>(this, "Start");

            MessagingCenter.Unsubscribe<MainPage>(this, "ToggleHome");

            MessagingCenter.Unsubscribe<MainPage>(this, "ViewStored");
        }

        private void PushPageBasedOnSelectedItem(DriveReportCellModel item)
        {
            switch (item.Name)
            {
                case purposeText:
                    Navigation.PushAsync<PurposeViewModel>();
                    break;
                case organisatoriskText:
                    Navigation.PushAsync<OrganizationViewModel>();
                    break;
                case takstText:
                    Navigation.PushAsync<TaxViewModel>();
                    break;
                case ekstraText:
                    Navigation.PushAsync<RemarkViewModel>();
                    break;
            }
        }

        #region Properties

        public const string DriveProperty = "DriveReportList";
        public ObservableCollection<DriveReportCellModel> DriveReportList
        {
            get { return driveReport; }
            set
            {
                driveReport = value;
                OnPropertyChanged(DriveProperty);
            }
        }

        public const string HomeCheckProperty = "HomeCheck";
        public bool HomeCheck
        {
            get
            {
                return homeCheck;
            }
            set
            {
                homeCheck = value;
                Definitions.Report.StartsAtHome = value;
                OnPropertyChanged(HomeCheckProperty);
            }
        }


        private void InitializeCollection()
        {
            driveReport.Clear();
            DriveReportList.Clear();

            string remarkText;
            if (Definitions.Report.ManualEntryRemark == "" || Definitions.Report.ManualEntryRemark == null)
            {
                remarkText = "Indtast Bemærkning";                
            }
            else
            {
                remarkText = Definitions.Report.ManualEntryRemark;
            }
            
            driveReport.Add(new DriveReportCellModel
            {
                Name = purposeText,
                Description = Definitions.Purpose,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = organisatoriskText,
                Description = Definitions.Organization.EmploymentPosition,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = takstText,
                Description = Definitions.Taxe.Description,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = ekstraText,
                Description = remarkText,
            });

            DriveReportList = driveReport;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
