using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services.Geolocation;


namespace OS2Indberetning.ViewModel
{
    public class UploadingViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string uploaderText;
        private double rotate;

        private bool timerContinue = true;

        public UploadingViewModel()
        {
            UploaderText = "Uploader kørselsdata";

            Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
            {

                if (timerContinue)
                {
                    Rotate = Rotate + 25;
                    return true;
                }
                return false; //not continue

            });
        }

        private void Subscribe()
        {

        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<GpsPage>(this, "Toggle");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Finish");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Stop");
            MessagingCenter.Unsubscribe<GpsPage>(this, "ToggleFinishedHome");
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
