using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;

using OS2Indberetning.Model;


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
        private string newKm;
        
        private const string purposeText = "Formål: ";
        private const string organisatoriskText = "Organisatorisk placering:";
        private const string takstText = "Takst";
        private const string ekstraText = "Ekstra Bemærkning:";
        private const string kilometerText = "Antal Km:";

        public FinishDriveViewModel()
        {
            driveReport = new ObservableCollection<DriveReportCellModel>();
            NewKm = Definitions.Report.Route.TotalDistance.ToString();

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
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Delete", (sender) => HandleDeleteMessage(sender));
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Selected", (sender) =>
            {
                PushPageBasedOnSelectedItem(sender);
            });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Update", (sender) =>
            {
                InitializeCollection();
            });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "EndHome",(sender) => { StartHomeCheck = !StartHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "StartHome", (sender) => { EndHomeCheck = !EndHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "NewKm", (sender) =>
            {
                try
                {
                    Definitions.Report.Route.TotalDistance = Convert.ToDouble(newKm);
                }
                catch (Exception e)
                {
                    // ONLY happens if user somehow writes letters with numeric keyboard?  
                    // Can happen in a simulator
                }
                InitializeCollection();
                sender._PopUpLayout.DismissPopup();
            });
        }

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

        private void PushPageBasedOnSelectedItem(FinishDrivePage sender)
        {
            DriveReportCellModel item = sender.list.SelectedItem as DriveReportCellModel;

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
                case kilometerText:
                    sender._PopUpLayout.ShowPopup(sender.EditKmPopup());
                    sender.list.SelectedItem = null;
                    break;
            }
        }

        private void HandleDeleteMessage(object sender)
        {
            // Doing some cleanup
            Definitions.Report = new DriveReport();
            // Popping to mainpage
            var stack = (sender as FinishDrivePage).Nav.NavigationStack;
            for (int i = 2; i < stack.Count; )
            {
                if (stack.Count == 3) break;
                (sender as FinishDrivePage).Nav.RemovePage(stack[i]);
            }
            Dispose();
            Navigation.PopAsync();
        }

        private void UploadHandler()
        {
            Dispose();
            Navigation.PushAsync<UploadingViewModel>();
        }

        private void InitializeCollection()
        {
            driveReport.Clear();
            DriveReportList.Clear();

            driveReport.Add(new DriveReportCellModel
            {
                Name = purposeText,
                Description = Definitions.Report.Purpose,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = organisatoriskText,
                Description = Definitions.Report.Profile.Employments.FirstOrDefault(x => x.Id == Definitions.Report.EmploymentId).EmploymentPosition,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = takstText,
                Description = Definitions.Report.Rate.Description,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = ekstraText,
                Description = Definitions.Report.ManualEntryRemark,
            });
            driveReport.Add(new DriveReportCellModel
            {
                Name = kilometerText,
                Description = Definitions.Report.Route.TotalDistance.ToString(),
            });

            DriveReportList = driveReport;
        }

        #region properties
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
                Definitions.Report.StartsAtHome = value;
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
                Definitions.Report.EndsAtHome = value;
                OnPropertyChanged(EndHomeCheckProperty);
            }
        }

        public const string NewKmProperty = "NewKm";
        public string NewKm
        {
            get { return newKm; }
            set
            {
                newKm = value;
                OnPropertyChanged(NewKmProperty);
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

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
