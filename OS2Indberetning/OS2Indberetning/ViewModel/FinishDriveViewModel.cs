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
    public class FinishDriveViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<DriveReportCellModel> driveReport;

        private bool startHomeCheck;
        private bool endHomeCheck;
        private string date;
        private string username;

        public FinishDriveViewModel()
        {
            driveReport = new ObservableCollection<DriveReportCellModel>();

            InitializeCollection();

            Username = Definitions.Report.Profile.FirstName + " " + Definitions.Report.Profile.LastName;
            Date = "Dato: " + Definitions.Report.Date;
            StartHomeCheck = Definitions.Report.StartsAtHome;
            EndHomeCheck = Definitions.Report.EndsAtHome;
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
            driveReport = null;
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Upload", (sender) => UploadHandler());
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Upload");
        }

        private void UploadHandler()
        {
            Navigation.PushAsync<UploadingViewModel>();
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

        public const string StartHomeCheckProperty = "StartHomeCheck";
        public bool StartHomeCheck
        {
            get
            {
                return startHomeCheck;
            }
            set
            {
                startHomeCheck = value;
                OnPropertyChanged(StartHomeCheckProperty);
            }
        }

        public const string EndHomeCheckProperty = "EndHomeCheck";
        public bool EndHomeCheck
        {
            get
            {
                return endHomeCheck;
            }
            set
            {
                endHomeCheck = value;
                OnPropertyChanged(EndHomeCheckProperty);
            }
        }

        public const string DateProperty = "Date";
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                OnPropertyChanged(DateProperty);
            }
        }

        public const string UsernameProperty = "Username";
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged(EndHomeCheckProperty);
            }
        }


        private void InitializeCollection()
        {
            driveReport.Clear();
            DriveReportList.Clear();
            
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Formål:",
                Description = Definitions.Report.Purpose,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Organisatorisk placering:",
                Description = Definitions.Report.Profile.Employments.FirstOrDefault(x => x.Id == Definitions.Report.EmploymentId).EmploymentPosition,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Takst",
                Description = Definitions.Report.Rate.Description,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Ekstra Bemærkning:",
                Description = Definitions.Report.ManualEntryRemark,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = "Antal Km:",
                Description = Definitions.Report.Route.TotalDistance.ToString(),
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
