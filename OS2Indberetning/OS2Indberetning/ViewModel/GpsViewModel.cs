using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms;

using OS2Indberetning.Model;
using XLabs.Platform.Services.Geolocation;


namespace OS2Indberetning.ViewModel
{
    public class GpsViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
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

        public void Dispose()
        {
            Unsubscribe();
            StopGps();
            if (cancelSource != null)
            {
                cancelSource.Dispose();
            }
            locator = null;
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<GpsPage>(this, "Toggle", (sender) => ToggleGps());
            MessagingCenter.Subscribe<GpsPage>(this, "Finish", (sender) => FinishDrive());
            MessagingCenter.Subscribe<GpsPage>(this, "Back", (sender) => HandleBackMessage());
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

        private void HandleBackMessage()
        {
            //Dispose();
            App.Navigation.PopToRootAsync();
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
                if (Definitions.Route.GPSCoordinates.Count > 0)
                {
                    Definitions.Route.GPSCoordinates.Last().IsViaPoint = "true";
                } 
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
        }

        public void FinishDrive()
        {
            Definitions.Report.EmploymentId = Definitions.Organization.Id;
            Definitions.Report.Date = Definitions.Date;

            Definitions.Report.ProfileId = Definitions.User.Profile.Id;
            Definitions.Report.RateId = Definitions.Taxe.Id;
            Definitions.Report.Profile = Definitions.User.Profile;
            Definitions.Report.Rate = Definitions.Taxe;
            Definitions.Report.Route = Definitions.Route;
            Definitions.Report.Route.TotalDistance = TraveledDistance;

            Dispose();
            Navigation.PushAsync<FinishDriveViewModel>();
        }

        public void GpsNotAvailable()
        {
            Accuracy = "Gps er ikke tilgængelig";
        }

        public void PositionChanged(object sender, PositionEventArgs e)
        {
            IsBusy = true;
            locator.GetPositionAsync(timeout: 5000, cancelToken: this.cancelSource.Token, includeHeading: false)
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
                        TraveledDistance = Math.Round(GpsCalculator.Distance(positionLatitude, positionLongitude, t.Result.Latitude, t.Result.Longitude, 'K'), 3);
                        positionLatitude = t.Result.Latitude;
                        positionLongitude = t.Result.Longitude;
                        PositionStatus = t.Result.Timestamp.ToString("HH:mm:ss");
                        Definitions.Route.GPSCoordinates.Add(new GPSCoordinate
                        {
                            Latitude = t.Result.Latitude.ToString("#.######", CultureInfo.InvariantCulture),
                            Longitude = t.Result.Longitude.ToString("#.######", CultureInfo.InvariantCulture),
                        });
                    }

                });
        }

        public void SetupGps()
        {
            locator = DependencyService.Get<IGeolocator>();
            if (locator.DesiredAccuracy != Definitions.Accuracy)
            {
                locator.DesiredAccuracy = Definitions.Accuracy;
            }
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
                Definitions.Report.EndsAtHome = value;
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
