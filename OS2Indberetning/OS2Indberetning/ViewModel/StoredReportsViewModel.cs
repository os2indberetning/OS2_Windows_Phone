using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms;

using OS2Indberetning.Model;
using XLabs.Platform.Services;


namespace OS2Indberetning.ViewModel
{
    public class StoredReportsViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<StoredReportCellModel> storedList;

        private Token token;
        private ISecureStorage storage;

        public StoredReportsViewModel()
        {
            storedList = new ObservableCollection<StoredReportCellModel>();

            storage = DependencyService.Get<ISecureStorage>();
            var tokenByte = storage.Retrieve(Definitions.TokenKey);

            token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(tokenByte, 0, tokenByte.Length));

            ReportListHandler.GetReportList().ContinueWith((result) => { InitializeCollection(result.Result); });
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
            storedList = null;
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<StoredReportsPage>(this, "Back", (sender) =>
            {
                Dispose();
                Navigation.PopAsync();
            });

            MessagingCenter.Subscribe<StoredReportsPage>(this, "Upload", (sender) =>
            {
                var item =  (StoredReportCellModel)sender.list.SelectedItem;
                
                APICaller.SubmitDrive(item.report, token, Definitions.MunUrl).ContinueWith((result) =>
                {
                    HandleUploadResult(result.Result, sender);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });


            MessagingCenter.Subscribe<StoredReportsPage>(this, "Remove", (sender) =>
            {
                var item = (StoredReportCellModel)sender.list.SelectedItem;

                RemoveItemFromList(item, sender);
            });
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Back");
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Upload");
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Remove");
        }

        private void HandleUploadResult(UserInfoModel item, StoredReportsPage page)
        {
            if (item == null)
            {
                page.ClosePopup();
                var popup = page.CreateErrorPopup("Kunne ikke upload på nuværende tidspunkt. Prøv igen senere");
                page._PopUpLayout.ShowPopup(popup);
                return;
            }
            else
            {
                RemoveItemFromList((StoredReportCellModel)page.list.SelectedItem, page);
            }
        }

        private void RemoveItemFromList(StoredReportCellModel item, StoredReportsPage page)
        {
            ReportListHandler.RemoveReportFromList(item.report).ContinueWith((result) =>
            {
                page.ClosePopup();
                InitializeCollection(result.Result);
                var popup = page.CreateErrorPopup("Kørsels rapport blev uploaded");
                page._PopUpLayout.ShowPopup(popup);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitializeCollection(List<DriveReport> list)
        {
            storedList.Clear();
            StoredList.Clear();

            var datePre = "Rapporteret den ";
            var distancePre = "Distance: ";
            var purposePre = "Formål: ";
            var taxePre = "Takst: ";

            foreach (var item in list)
            {
                storedList.Add(new StoredReportCellModel
                {
                    Date = datePre + item.Date,
                    Distance = distancePre + item.Route.TotalDistance.ToString() + " km",
                    Purpose = purposePre + item.Purpose,
                    Taxe = item.Rate.Description,
                    report = item,
                });
            }

            StoredList = storedList;
        }

        #region Properties
        public const string ListProperty = "StoredList";
        public ObservableCollection<StoredReportCellModel> StoredList
        {
            get { return storedList; }
            set
            {
                storedList = value;
                OnPropertyChanged(ListProperty);
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
