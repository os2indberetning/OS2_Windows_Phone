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
    public class GpsViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string accuracy;
        
        private string driven;
        private string lastUpdate;

        private IGeolocator locator;

        private string positionStatus;
        private double positionLatitude;
        private double positionLongitude;

        private double distance;
        private bool firstRun = true;
        private bool finishedHome = false;
        private bool pause = true;

        private CancellationTokenSource cancelSource;
        

        public GpsViewModel()
        {
            TraveledDistance = 0;
            PositionStatus = DateTime.Now.ToString("HH:mm:ss");
            Accuracy = "GPS Nøjagtighed: " + Convert.ToString(Definitions.Accuracy) + " m";
            Subscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<GpsPage>(this, "Toggle", (sender) => ToggleGps());
            MessagingCenter.Subscribe<GpsPage>(this, "Finish", (sender) => FinishDrive());
            MessagingCenter.Subscribe<GpsPage>(this, "Closing", (sender) => StopGps());
            MessagingCenter.Subscribe<GpsPage>(this, "ToggleFinishedHome", (sender) => {
                FinishedHome = !FinishedHome;
                Definitions.EndsAtHome = FinishedHome;
            });
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<GpsPage>(this, "Toggle");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Finish");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Stop");
            MessagingCenter.Unsubscribe<GpsPage>(this, "ToggleFinishedHome");
        }
       
        public void ToggleGps()
        {
            if (locator == null)
            {
                SetupGps();
            }

            pause = !pause;
            if (!pause)
            {
                var test = locator.IsGeolocationAvailable;
                if (!test)
                {
                    GpsNotAvailable();
                }
                else
                {
                    
                    locator.StartListening(Definitions.MinInterval, Definitions.Accuracy, true);
                }
            }
            else
            {
                locator.StopListening();
            }
        }

        public void StopGps()
        {
            if (locator != null)
            {
                if (locator.IsGeolocationEnabled)
                {
                    locator.StopListening();
                    cancelSource.Cancel(false);
                    locator.PositionChanged -= PositionChanged;
                }
            }
            Unsubscribe();
        }

        public void FinishDrive()
        {
            var report = new DriveReport();

            report.EmploymentId = Definitions.Organization.Id;
            report.Date = Definitions.Date;
            report.ManualEntryRemark = Definitions.Remark;
            report.Purpose = Definitions.Purpose;
            report.StartsAtHome = Definitions.StartAtHome;
            report.EndsAtHome = Definitions.EndsAtHome;

            report.Profile = Definitions.User.Profile;
            report.Rate = Definitions.Taxe;

            report.Route = Definitions.Route;
            report.Route.TotalDistance = TraveledDistance;

            Definitions.Report = report;

            Navigation.PushAsync((ContentPage)ViewFactory.CreatePage<FinishDriveViewModel, FinishDrivePage>());
        }

        public void GpsNotAvailable()
        {
            Accuracy = "Gps er ikke tilgængelig";
        }

        public void PositionChanged(object sender, PositionEventArgs e)
        {
            IsBusy = true;
            locator.GetPositionAsync(timeout: 10000, cancelToken: this.cancelSource.Token, includeHeading: true)
                .ContinueWith(t =>
                {
                    IsBusy = false;
                    if (t.IsFaulted)
                        PositionStatus = ((GeolocationException)t.Exception.InnerException).Error.ToString();
                    else if (t.IsCanceled)
                        PositionStatus = "Canceled";
                    else
                    {
                        if (firstRun)
                        {
                            positionLatitude = t.Result.Latitude;
                            positionLongitude = t.Result.Longitude;
                            firstRun = false;
                        }
                        TraveledDistance = GpsCalculator.Distance(positionLatitude, positionLongitude, t.Result.Latitude, t.Result.Longitude, 'K');
                        positionLatitude = t.Result.Latitude;
                        positionLongitude = t.Result.Longitude;
                        PositionStatus = t.Result.Timestamp.ToString("HH:mm:ss");
                        Definitions.Route.GPSCoordinates.Add(new GPSCoordinate
                        {
                            Latitude = t.Result.Latitude.ToString(),
                            Longtitude = t.Result.Longitude.ToString(),
                        });
                    }

                });
        }

        public void SetupGps()
        {
            locator = DependencyService.Get<IGeolocator>();
            locator.DesiredAccuracy = Definitions.Accuracy;
            cancelSource = new CancellationTokenSource();
            locator.PositionChanged += PositionChanged;
        }

        public void SetAccuracy()
        {
            if (pause)
            {
                Accuracy = "GPS sat på pause";
            }
            else
            {
                Accuracy = "GPS Nøjagtighed: " + Convert.ToString(Definitions.Accuracy) + " m";
            }

        }

        public string PositionStatus
        {
            get
            {
                return positionStatus;
            }
            set
            {
                positionStatus = value;
                LastUpdate = "Sidst opdateret kl: " + value;
            }
        }

        public double TraveledDistance
        {
            get { return distance; }
            set
            {
                distance = distance + value;
                Driven = Convert.ToString(Math.Round(distance, 2));
            }
        }

        #region properties
        public const string AccuracyProperty = "Accuracy";
        public string Accuracy
        {
            get
            {
                return accuracy;
            }
            set
            {
                accuracy = value;
                OnPropertyChanged(AccuracyProperty);
            }
        }

        public const string FinishedHomeProperty = "FinishedHome";
        public bool FinishedHome
        {
            get
            {
                return finishedHome;
            }
            set
            {
                finishedHome = value;
                OnPropertyChanged(FinishedHomeProperty);
            }
        }

        public const string PauseProperty = "Pause";
        public bool Pause
        {
            get
            {
                return pause;
            }
            set
            {
                pause = value;
                OnPropertyChanged(PauseProperty);
            }
        }

        public const string DrivenProperty = "Driven";
        public string Driven
        {
            get
            {
                return driven;
            }
            set
            {
                driven = "Du har nu kørt " + value + " km";
                OnPropertyChanged(DrivenProperty);
            }
        }

        public const string LastUpdateProperty = "LastUpdate";
        public string LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                lastUpdate = value;
                OnPropertyChanged(LastUpdateProperty);
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
