using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Geolocation;


namespace OS2Indberetning.ViewModel
{
    public class UploadingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string uploaderText;
        private double rotate;
        private ISecureStorage storage;

        private bool timerContinue;
        private bool errorVisibility;
        private bool uploadingVisibility;

        private string errorText;

        private Token token;

        public UploadingViewModel()
        {
            UploaderText = "Uploader kørselsdata";
            storage = DependencyService.Get<ISecureStorage>();
            var tokenByte = storage.Retrieve(Definitions.TokenKey);

            token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(tokenByte, 0, tokenByte.Length));

            Subscribe();

            Upload();
        }

        public void Upload()
        {
            UploadingVisibility = true;
            ErrorVisibility = false;
            timerContinue = true;
            RotateSpinner();
            APICaller.SubmitDrive(Definitions.Report, token, Definitions.MunUrl).ContinueWith((result) =>
            {
                HandleUploadResult(result.Result);
            });
        }

        public void Dispose()
        {
            timerContinue = false;
            Unsubscribe();
            storage = null;
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<UploadingPage>(this, "Upload", (sender) => { Upload(); });

            MessagingCenter.Subscribe<UploadingPage>(this, "Store", (sender) => { HandleSaving(sender); });
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<UploadingPage>(this, "Upload");

            MessagingCenter.Unsubscribe<UploadingPage>(this, "Save");
        }

        private void HandleUploadResult(UserInfoModel user)
        {
            if (user == null)
            {
                ErrorText =
                    "Der skete en fejl ved afsendelsen af din rapport!" +
                    " Prøv igen eller tryk på 'Gem' og send rapporten fra hovedmenuen på et andet tidspunkt.";
                UploadingVisibility = false;
                timerContinue = false;
                ErrorVisibility = true;
            }
            else
            {
                
            }

            
            
        }

        private void HandleSaving(object sender)
        {
            ReportListHandler.AddReportToList(Definitions.Report).ContinueWith((result) =>
            {
                if (result != null)
                {
                    if (result.Result == true)
                    {
                        var stack = (sender as UploadingPage).Nav.NavigationStack;
                        for (int i = 2; i < stack.Count; )
                        {
                            (sender as UploadingPage).Nav.RemovePage(stack[i]);
                        }
                        //Navigation.PopToRootAsync();
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RotateSpinner()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0.05), () =>
            {
                if (timerContinue)
                {
                    Rotate = Rotate + 25;
                    return true;
                }
                return false; //not continue
            });
        }
      
        #region properties
        public const string UploaderTextProperty = "UploaderText";
        public string UploaderText
        {
            get
            {
                return uploaderText;
            }
            set
            {
                uploaderText = value;
                OnPropertyChanged(UploaderTextProperty);
            }
        }

        public const string ErrorVisibilityProperty = "ErrorVisibility";
        public bool ErrorVisibility
        {
            get
            {
                return errorVisibility;
            }
            set
            {
                errorVisibility = value;
                OnPropertyChanged(ErrorVisibilityProperty);
            }
        }


        public const string UploadingVisibilityProperty = "UploadingVisibility";
        public bool UploadingVisibility
        {
            get
            {
                return uploadingVisibility;
            }
            set
            {
                uploadingVisibility = value;
                OnPropertyChanged(UploadingVisibilityProperty);
            }
        }

        public const string ErrorTextProperty = "ErrorText";
        public string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
                OnPropertyChanged(ErrorTextProperty);
            }
        }

        public const string RotateProperty = "Rotate";
        public double Rotate
        {
            get
            {
                return rotate;
            }
            set
            {
                rotate = value;
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
