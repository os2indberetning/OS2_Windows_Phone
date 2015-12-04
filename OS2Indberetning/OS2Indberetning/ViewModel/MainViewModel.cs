using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using XLabs.Forms.Mvvm;


namespace OS2Indberetning.ViewModel
{
    public class MainViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<DriveReportCellModel> driveReport;
        private bool homeCheck = false;

        public MainViewModel()
        {
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
                Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<GpsViewModel, GpsPage>());
            });

            MessagingCenter.Subscribe<MainPage>(this, "ToggleHome", (sender) =>
            {
                HomeCheck = !HomeCheck;
                Definitions.StartAtHome = HomeCheck;
            });
        }

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
                Name = "Formål:",
                Description = Definitions.Purpose,
                Page = (ContentPage)(ContentPage)ViewFactory.CreatePage<PurposeViewModel, PurposePage>()
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Organisatorisk placering:",
                Description = Definitions.Organization.EmploymentPosition,
                Page = (ContentPage)(ContentPage)ViewFactory.CreatePage<OrganizationViewModel, OrganizationPage>()
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Takst",
                Description = Definitions.Taxe.Description,
                Page = (ContentPage)(ContentPage)ViewFactory.CreatePage<TaxViewModel, TaxPage>()
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Ekstra Bemærkning:",
                Description = remarkText,
                Page = (ContentPage)(ContentPage)ViewFactory.CreatePage<RemarkViewModel, RemarkPage>()
            });

            DriveReportList = driveReport;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
