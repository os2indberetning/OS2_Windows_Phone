/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
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
    /// Viewmodel of the Uploading page. Handles all view logic
    /// </summary>
    public class UploadingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _uploaderText;
        private double _rotate;
        private ISecureStorage _storage;
        private readonly double _minimumWait = 3;
        private bool _timerContinue;
        private bool _errorVisibility;
        private bool _uploadingVisibility;

        private string _errorText;
        private readonly Token _token;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public UploadingViewModel()
        {
            UploaderText = "Uploader kørselsdata";
            _storage = DependencyService.Get<ISecureStorage>();
            var tokenByte = _storage.Retrieve(Definitions.TokenKey);

            _token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(tokenByte, 0, tokenByte.Length));

            Subscribe();
        }
        
        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            _timerContinue = false;
            Unsubscribe();
            _storage = null;
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<UploadingPage>(this, "Upload", HandleUploadMessage);

            MessagingCenter.Subscribe<UploadingPage>(this, "Store", HandleStoreMessage);
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<UploadingPage>(this, "Upload");

            MessagingCenter.Unsubscribe<UploadingPage>(this, "Store");
        }

        /// <summary>
        /// Method that handles the Upload Result
        /// </summary>
        private void HandleUploadResult(ReturnUserModel user, object sender)
        {
            Device.StartTimer(TimeSpan.FromSeconds(_minimumWait), () =>
            {
                if (user.User == null)
                {
                    ErrorText =
                        "Der skete en fejl ved afsendelsen af din rapport!" + "\nFejl: " + user.Error.ErrorMessage +
                        "\nPrøv igen eller tryk på 'Gem' og send rapporten fra hovedmenuen på et andet tidspunkt.";
                    UploadingVisibility = false;
                    _timerContinue = false;
                    ErrorVisibility = true;
                }
                else
                {
                    Dispose();
                    App.Navigation.PopToRootAsync();
                }
                return false; //not continue
            });
        }

        /// <summary>
        /// Method that handles the rotation of the spinner
        /// </summary>
        private void RotateSpinner()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0.05), () =>
            {
                if (_timerContinue)
                {
                    Rotate = Rotate + 25;
                    return true;
                }
                return false; //not continue
            });
        }

        /// <summary>
        /// Method that handles the Upload message
        /// </summary>
        public void HandleUploadMessage(UploadingPage sender)
        {
            // for testing
            //HandleUploadResult(null, null);
            //return;

            UploadingVisibility = true;
            ErrorVisibility = false;
            _timerContinue = true;
            RotateSpinner();
            APICaller.SubmitDrive(Definitions.Report, _token, Definitions.MunUrl).ContinueWith((result) =>
            {
                HandleUploadResult(result.Result, sender);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Method that handles the Store message
        /// </summary>
        private void HandleStoreMessage(UploadingPage sender)
        {
            ReportListHandler.AddReportToList(Definitions.Report).ContinueWith((result) =>
            {
                if (result != null)
                {
                    if (result.Result == true)
                    {
                        Dispose();
                        App.Navigation.PopToRootAsync();
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
      
        #region properties
        public const string UploaderTextProperty = "UploaderText";
        public string UploaderText
        {
            get
            {
                return _uploaderText;
            }
            set
            {
                _uploaderText = value;
                OnPropertyChanged(UploaderTextProperty);
            }
        }

        public const string ErrorVisibilityProperty = "ErrorVisibility";
        public bool ErrorVisibility
        {
            get
            {
                return _errorVisibility;
            }
            set
            {
                _errorVisibility = value;
                OnPropertyChanged(ErrorVisibilityProperty);
            }
        }


        public const string UploadingVisibilityProperty = "UploadingVisibility";
        public bool UploadingVisibility
        {
            get
            {
                return _uploadingVisibility;
            }
            set
            {
                _uploadingVisibility = value;
                OnPropertyChanged(UploadingVisibilityProperty);
            }
        }

        public const string ErrorTextProperty = "ErrorText";
        public string ErrorText
        {
            get
            {
                return _errorText;
            }
            set
            {
                _errorText = value;
                OnPropertyChanged(ErrorTextProperty);
            }
        }

        public const string RotateProperty = "Rotate";
        public double Rotate
        {
            get
            {
                return _rotate;
            }
            set
            {
                _rotate = value;
                OnPropertyChanged(RotateProperty);
            }
        }

        public const string EmblemProperty = "Emblem";
        public ImageSource Emblem
        {
            get
            {
                return Definitions.MunIcon;
            }
        }

        public const string SpinnerProperty = "Spinner";
        public ImageSource Spinner
        {
            get
            {
                return "Resources/spinner.png";
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
