using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OS2Indberetning.BuisnessLogic;
using Xamarin.Forms;

using OS2Indberetning.Model;
using XLabs.Platform.Services.Geolocation;


namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the GPS page. Handles all view logic
    /// </summary>
    public class GpsViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GpsPage _parentPage;

        private string _accuracy;
        
        private string _driven;
        private string _lastUpdate;

        private IGeolocator _locator;

        private string _positionStatus;
        private double _positionLatitude;
        private double _positionLongitude;

        private double pauseLatitude;
        private double pauseLongitude;

        private double _distance;
        private bool _firstRun = true;
        private bool _finishedHome = false;
        private bool _pause = true;

        private CancellationTokenSource _cancelSource;

        private int _countDown = 5;
        private bool _trackChanges = false;
        private bool _doTestCheck;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public GpsViewModel()
        {
            TraveledDistance = 0;
            PositionStatus = DateTime.Now.ToString("HH:mm:ss");
            Accuracy = "GPS Nøjagtighed: " + Convert.ToString(Definitions.Accuracy) + " m";
            Subscribe();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            _parentPage.Dispose();
            Unsubscribe();
            HandleStopGpsMessage();
            if (_cancelSource != null)
            {
                _cancelSource.Dispose();
            }
            _locator = null;
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<GpsPage>(this, "Toggle", (sender) =>
            {
                _parentPage = sender;
                HandleToggleGpsMessage();
            });
            MessagingCenter.Subscribe<GpsPage>(this, "Finish", (sender) =>
            {
                _parentPage = sender;
                HandleFinishDriveMessage();
            });
            MessagingCenter.Subscribe<GpsPage>(this, "Back", (sender) =>
            {
                _parentPage = sender;
                HandleBackMessage();
            });
            MessagingCenter.Subscribe<GpsPage>(this, "ToggleFinishedHome", (sender) => {
                FinishedHome = !FinishedHome;
                Definitions.EndsAtHome = FinishedHome;
            });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<GpsPage>(this, "Toggle");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Finish");
            MessagingCenter.Unsubscribe<GpsPage>(this, "Stop");
            MessagingCenter.Unsubscribe<GpsPage>(this, "ToggleFinishedHome");
        }

        /// <summary>
        /// Method that handles sending a PauseError
        /// </summary>
        public void PauseDistanceTooBig(double distance)
        {
            _parentPage.HandlePauseError();
        }

        /// <summary>
        /// Method to do stuff when gps is not available
        /// </summary>
        public void GpsNotAvailable()
        {
            Accuracy = "Gps er ikke tilgængelig";
        }

        /// <summary>
        /// Method that handles change in positions from the gps signal
        /// </summary>
        public void PositionChanged(object sender, PositionEventArgs e)
        {
            IsBusy = true;
            _locator.GetPositionAsync(timeout: (int)Definitions.MinInterval, cancelToken: this._cancelSource.Token, includeHeading: false)
                .ContinueWith(t =>
                {
                    IsBusy = false;
                    if (t.IsFaulted)
                        PositionStatus = ((GeolocationException)t.Exception.InnerException).Error.ToString();
                    else if (t.IsCanceled)
                        PositionStatus = "Canceled";
                    else
                    {
                        if (_trackChanges)
                        {
                            if (_firstRun)
                            {
                                _positionLatitude = t.Result.Latitude;
                                _positionLongitude = t.Result.Longitude;
                                _firstRun = false;
                                _doTestCheck = false;
                            }
                            if (_doTestCheck)
                            {
                                _doTestCheck = !_doTestCheck;
                                var testDistance = Math.Round(GpsCalculator.Distance(_positionLatitude, _positionLongitude, t.Result.Latitude, t.Result.Longitude, 'K'), 3);
                                if (testDistance > 0.2)
                                {
                                    _locator.StopListening();
                                    _pause = !_pause;
                                    PauseDistanceTooBig(testDistance);
                                    return;
                                }
                            }
                            TraveledDistance = Math.Round(GpsCalculator.Distance(_positionLatitude, _positionLongitude, t.Result.Latitude, t.Result.Longitude, 'K'), 3);
                            _positionLatitude = t.Result.Latitude;
                            _positionLongitude = t.Result.Longitude;
                            PositionStatus = t.Result.Timestamp.ToString("HH:mm:ss");
                            Definitions.Route.GPSCoordinates.Add(new GPSCoordinate
                            {
                                Latitude = t.Result.Latitude.ToString("#.######", CultureInfo.InvariantCulture),
                                Longitude = t.Result.Longitude.ToString("#.######", CultureInfo.InvariantCulture),
                            });
                        }
                    }

                });
        }

        /// <summary>
        /// Method that does the initial gps setup
        /// </summary>
        public void SetupGps()
        {
            _locator = DependencyService.Get<IGeolocator>();
            if (_locator.DesiredAccuracy != Definitions.Accuracy)
            {
                _locator.DesiredAccuracy = Definitions.Accuracy;
            }
            _cancelSource = new CancellationTokenSource();
            _locator.PositionChanged += PositionChanged;
            _locator.StartListening(Definitions.MinInterval, Definitions.Accuracy, false);
        }

        /// <summary>
        /// Method that does a countdown on the screen.
        /// Also makes sure the gps isnt tracked while countdown is active
        /// </summary>
        private void CountDown()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                LastUpdate = "Gps starter om: " + _countDown + " sekunder";
                if (_countDown > 0)
                {
                    _countDown = _countDown - 1;
                    return true;
                }

                _countDown = 5; // reset timer
                PositionStatus = DateTime.Now.ToString("HH:mm:ss");
                _trackChanges = true;
                return false; //not continue
            });
        }

        #region helper methods

        /// <summary>
        /// Method to help with setting the correct text for Accuracy
        /// </summary>
        public void SetAccuracy()
        {
            if (_pause)
            {
                Accuracy = "GPS sat på _pause";
            }
            else
            {
                Accuracy = "GPS Nøjagtighed: " + Convert.ToString(Definitions.Accuracy) + " m";
            }

        }

        /// <summary>
        /// Method to help with setting the correct text for LastUpdate
        /// </summary>
        public string PositionStatus
        {
            get
            {
                return _positionStatus;
            }
            set
            {
                _positionStatus = value;
                LastUpdate = "Sidst opdateret kl: " + value;
            }
        }

        /// <summary>
        /// Method to help with setting the correct text for Driven
        /// </summary>
        public double TraveledDistance
        {
            get { return _distance; }
            set
            {
                _distance = _distance + value;
                Driven = Convert.ToString(Math.Round(_distance, 2));
            }
        }

        #endregion

        #region Message Handlers

        /// <summary>
        /// Method that handles the Back message
        /// </summary>
        private void HandleBackMessage()
        {
            Dispose();
            App.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that handles the ToggleGps message
        /// </summary>
        public void HandleToggleGpsMessage()
        {
            if (_locator == null)
            {
                CountDown();
                SetupGps();
                return;
            }

            _pause = !_pause;
            if (_pause)
            {
                var test = _locator.IsGeolocationAvailable;
                if (!test)
                {
                    GpsNotAvailable();
                }
                else
                {
                    _doTestCheck = true;
                    CountDown();
                    _locator.StartListening(Definitions.MinInterval, Definitions.Accuracy);
                }
            }
            else
            {
                _locator.StopListening();
                pauseLatitude = _positionLatitude;
                pauseLongitude = _positionLongitude;
                if (Definitions.Route.GPSCoordinates.Count > 0)
                {
                    Definitions.Route.GPSCoordinates.Last().IsViaPoint = "true";
                    _trackChanges = true;
                }
            }
        }

        /// <summary>
        /// Method that handles the StopGps message
        /// </summary>
        public void HandleStopGpsMessage()
        {
            if (_locator != null)
            {
                if (_locator.IsGeolocationEnabled)
                {
                    _locator.StopListening();
                    _cancelSource.Cancel(false);
                    _locator.PositionChanged -= PositionChanged;
                }
            }
            _trackChanges = false;
        }

        /// <summary>
        /// Method that handles the FinishDrive message
        /// </summary>
        public void HandleFinishDriveMessage()
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

        #endregion

        #region properties
        public const string AccuracyProperty = "Accuracy";
        public string Accuracy
        {
            get
            {
                return _accuracy;
            }
            set
            {
                _accuracy = value;
                OnPropertyChanged(AccuracyProperty);
            }
        }

        public const string FinishedHomeProperty = "FinishedHome";
        public bool FinishedHome
        {
            get
            {
                return _finishedHome;
            }
            set
            {
                _finishedHome = value;
                Definitions.Report.EndsAtHome = value;
                OnPropertyChanged(FinishedHomeProperty);
            }
        }

        public const string PauseProperty = "Pause";
        public bool Pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
                OnPropertyChanged(PauseProperty);
            }
        }

        public const string DrivenProperty = "Driven";
        public string Driven
        {
            get
            {
                return _driven;
            }
            set
            {
                _driven = "Du har nu kørt " + value + " km";
                OnPropertyChanged(DrivenProperty);
            }
        }

        public const string LastUpdateProperty = "LastUpdate";
        public string LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
            set
            {
                _lastUpdate = value;
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
