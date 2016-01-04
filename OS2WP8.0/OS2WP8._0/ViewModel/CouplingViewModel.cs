using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using XLabs.Platform.Services;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Coupling page. Handles all view logic
    /// </summary>
    public class CouplingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Municipality _model;
        private ISecureStorage _storage;
        private string _token;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public CouplingViewModel()
        {
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
            MessagingCenter.Subscribe<CouplingPage>(this, "Couple", (sender) => { HandleCoupleMessage(); });
            MessagingCenter.Subscribe<CouplingPage>(this, "Back", (sender) =>
            {
                Dispose();
                App.Navigation.PopAsync();
            });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// Important this is called upon popping of the page
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<CouplingPage>(this, "Couple");
            MessagingCenter.Unsubscribe<CouplingPage>(this, "Back");
        }

        /// <summary>
        /// Method is used a a constructor, because the constructor needs to be parameterless
        /// </summary>
        public void InitVm(Municipality m)
        {
            _model = m;
            _storage = DependencyService.Get<ISecureStorage>();
        }

        /// <summary>
        /// Method that handles saving the user information if everything is OK
        /// </summary>
        /// <param name="user">UserInfoModel that is saved</param>
        /// <returns>True on success, false on failure</returns>
        private bool Couple(UserInfoModel user)
        {
            if (user == null)
            {
                App.ShowLoading(false, true);
                return false;
            }
            Definitions.User = user;
            var specificToken = user.Profile.Tokens.Find(x => x.TokenString == _token);
            if (specificToken == null)
            {
                App.ShowLoading(false, true);
                return false;
            }
            _storage.Store(Definitions.TokenKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(specificToken)));
            _storage.Store(Definitions.MunKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_model)));
            _storage.Store(Definitions.UserDataKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));
            return true;
        }

        #region Message Handlers
        /// <summary>
        /// Method that handles the couple message from the page
        /// </summary>
        private void HandleCoupleMessage()
        {
            string message;
            App.ShowLoading(true);
            APICaller.Couple(_model.APIUrl, _token).ContinueWith((result) =>
            {
                if(result.Result.User == null)
                {
                    App.ShowMessage("Parring fejlede\n" + "Fejl besked: " + result.Result.Error.ErrorMessage);
                    return;
                }

                var success = Couple(result.Result.User);
                App.ShowLoading(false, true);
                if (!success)
                {
                    App.ShowMessage("Parring fejlede\n" + "Fejl besked: Coupling Error");
                    return;
                }
                
                Dispose();
                App.Navigation.PopToRootAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Properties
        public const string TokenProperty = "Token";
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                OnPropertyChanged(TokenProperty);
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
