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
    /// <summary>
    /// Viewmodel of the StoredReports page. Handles all view logic
    /// </summary>
    public class StoredReportsViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string _datePre = "Rapporteret den ";
        private readonly string  _distancePre = "Distance: ";
        private readonly string _purposePre = "Formål: ";
        private readonly string _taxePre = "Takst: ";

        private ObservableCollection<StoredReportCellModel> _storedList;

        private readonly Token _token;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public StoredReportsViewModel()
        {
            _storedList = new ObservableCollection<StoredReportCellModel>();

            var storage = DependencyService.Get<ISecureStorage>();
            var tokenByte = storage.Retrieve(Definitions.TokenKey);

            _token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(tokenByte, 0, tokenByte.Length));

            ReportListHandler.GetReportList().ContinueWith((result) => { InitializeCollection(result.Result); });
            Subscribe();
        }

        /// <summary>
        /// Destructor 
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
            _storedList = null;
        }

        /// <summary>
        /// Method that subscribes to nessecary calls
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<StoredReportsPage>(this, "Back", (sender) => { HandleBackMessage(); });

            MessagingCenter.Subscribe<StoredReportsPage>(this, "Upload", HandleUploadMessage);

            MessagingCenter.Subscribe<StoredReportsPage>(this, "Remove", HandleRemoveMessage);
        }

        /// <summary>
        /// Method that handles all unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Back");
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Upload");
            MessagingCenter.Unsubscribe<StoredReportsPage>(this, "Remove");
        }

        /// <summary>
        /// Method that handles the upload result
        /// </summary>
        /// <param name="user">the ReturnUserModel the api returned after upload attempt</param>
        /// <param name="page">the parent page, used to open popup</param>
        private void HandleUploadResult(ReturnUserModel model, StoredReportsPage page)
        {
            if (model.User == null)
            {
                page.ClosePopup();
                var popup = page.CreateMessagePopup("Kunne ikke upload på nuværende tidspunkt. Prøv igen senere\nFejl: " + model.Error.ErrorMessage);
                page.PopUpLayout.ShowPopup(popup);
                return;
            }
            else
            {
                var item = (StoredReportCellModel) page.List.SelectedItem;
                ReportListHandler.RemoveReportFromList(item.report).ContinueWith((result) =>
                {
                    page.ClosePopup();
                    InitializeCollection(result.Result);
                    var popup = page.CreateMessagePopup("Kørsels rapport blev uploadet");
                    page.PopUpLayout.ShowPopup(popup);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Method that removes an item from the stored list
        /// </summary>
        /// <param name="item">the StoredReportCellModel that needs to be removed</param>
        /// <param name="page">the parent page, used to open popup</param>
        private void RemoveItemFromList(StoredReportCellModel item, StoredReportsPage page)
        {
            ReportListHandler.RemoveReportFromList(item.report).ContinueWith((result) =>
            {
                page.ClosePopup();
                InitializeCollection(result.Result);
                var popup = page.CreateMessagePopup("Kørsels rapport blev slettet");
                page.PopUpLayout.ShowPopup(popup);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        /// <param name="list">the list used to initialize the observerable collection</param>
        private void InitializeCollection(List<DriveReport> list)
        {
            _storedList.Clear();
            StoredList.Clear();

            foreach (var item in list)
            {
                _storedList.Add(new StoredReportCellModel
                {
                    Date = _datePre + item.Date,
                    Distance = _distancePre + item.Route.TotalDistance.ToString() + " km",
                    Purpose = _purposePre + item.Purpose,
                    Taxe = _taxePre + item.Rate.Description,
                    report = item,
                });
            }

            StoredList = _storedList;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles a Back message from the page
        /// Calls dispose and pops to root
        /// </summary>
        private void HandleBackMessage()
        {
            Dispose();
            App.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that handles the Upload message
        /// </summary>
        private void HandleUploadMessage(StoredReportsPage sender)
        {
            var item = (StoredReportCellModel)sender.List.SelectedItem;

            APICaller.SubmitDrive(item.report, _token, Definitions.MunUrl).ContinueWith((result) =>
            {
                HandleUploadResult(result.Result, sender);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Method that handles the Remove message
        /// </summary>
        private void HandleRemoveMessage(StoredReportsPage sender)
        {
            var item = (StoredReportCellModel)sender.List.SelectedItem;

            RemoveItemFromList(item, sender);
        }

        #endregion

        #region Properties
        public const string ListProperty = "StoredList";
        public ObservableCollection<StoredReportCellModel> StoredList
        {
            get { return _storedList; }
            set
            {
                _storedList = value;
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
