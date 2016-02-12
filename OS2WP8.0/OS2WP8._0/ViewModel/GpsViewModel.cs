/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
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

        private double _pauseLatitude;
        private double _pauseLongitude;

        private double _distance;
        private bool _firstRun = true;
        private bool _finishedHome = false;
        private bool _pause = false;
        private bool _signal;

        private CancellationTokenSource _cancelSource;

        private double _countDown = Definitions.GpsCountDown;
        private double _noSignalCountDown;
        private bool _trackChanges = false;
        private bool _error;
        private DateTime _errorDateTime;
        private bool _errorCountdownStarted;
        private bool _doTestCheck;
        private bool _isListening = false;
        private bool _stopErrorTimer;
        private bool _doCountdown;
        private bool _noGpsForTooLong = false;

        private string _lastAccuracy = "10";
        
        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public GpsViewModel()
        {
            Definitions.GpsIsActive = true;
            TraveledDistance = 0;
            PositionStatus = DateTime.Now.ToString("HH:mm:ss");
            SetAccuracy();
            Subscribe();
            SetupGps();
            _noSignalCountDown = Definitions.NoGpsSignalTimer;
            Timer(); // start timer
            /*TestGpsSignalTimer();*/ // timer that checks for gps signal when on pause
            TestForAvailibility();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            Definitions.GpsIsActive = false;
            _parentPage.Dispose();
            Unsubscribe();
            HandleStopGpsMessage();
            _stopErrorTimer = true; // stop timer;
            if (_cancelSource != null)
            {
                _cancelSource.Cancel();
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
            MessagingCenter.Subscribe<GpsPage>(this, "Here", (sender) =>
            {
                _parentPage = sender;
            });
            MessagingCenter.Subscribe<App>(this, "Appeared", HandleAppearedMessage);
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
            MessagingCenter.Unsubscribe<GpsPage>(this, "Here");
            MessagingCenter.Unsubscribe<App>(this, "Appeared");
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
            Signal = true;
            _error = true;
            if (!_doCountdown)
            {
                _errorDateTime = DateTime.Now;
            }
            
            _doCountdown = true;
            SetAccuracy();
        }

        /// <summary>
        /// Method to do stuff when gps is not available
        /// </summary>
        public void GpsIsAvailable()
        {
            Signal = false;
            _error = false;
            _noSignalCountDown = Definitions.NoGpsSignalTimer; // reset countdown

            _doCountdown = false;
            SetAccuracy();
        }

        /// <summary>
        /// Method that handles that tracks if gps signal have been missing for 30 seconds
        /// </summary>
        public void Timer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                // if flag already set, return
                if (_noGpsForTooLong) return true;

                _errorCountdownStarted = true;
                if (_doCountdown)
                {
                    if (_noSignalCountDown > 0)
                    {
                        // dont count down while on pause
                        if (!_pause)
                            return true;

                        PositionChanged(null, null);
                        _noSignalCountDown += -3;
                        return true;
                    }
                    _noGpsForTooLong = true;
                    return false;
                }

                if(_stopErrorTimer)
                   return false; //not continue

                _noSignalCountDown = Definitions.NoGpsSignalTimer;
                _errorCountdownStarted = false;
                return true; // continue 
            });
        }

        /// <summary>
        /// Method that checks for gps signal when the gps in on pause
        /// The cause of this is to notify the user of gps signal even when he isnt tracking
        /// </summary>
        public void TestGpsSignalTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (_stopErrorTimer)
                    return false; //not continue

                if (_pause)
                    return true; // dont test when NOT paused

                TestForAvailibility();

                return true; // continue 
            });
        }

        /// <summary>
        /// Method that handles change in positions from the gps signal
        /// </summary>
        public void PositionChanged(object sender, PositionEventArgs e)
        {
            IsBusy = true;
            if (_locator != null)
            {
                _locator.GetPositionAsync(timeout: Definitions.MinIntervalTimeout, cancelToken: this._cancelSource.Token, includeHeading: false)
                    .ContinueWith(t =>
                    {
                        IsBusy = false;
                        if (t.IsFaulted)
                        {
                            GpsNotAvailable();
                            //PositionStatus = ((GeolocationException)t.Exception.InnerException).Error.ToString();
                        }
                        else if (t.IsCanceled)
                            PositionStatus = "Canceled";
                        else
                        {
                            if (t.Result.Accuracy < 100)
                            {
                                GpsIsAvailable();
                            }
                            else
                            {
                                GpsNotAvailable();
                                return;
                            }
                            
                            if (_trackChanges)
                            {
                                if (_firstRun)
                                {
                                    _positionLatitude = t.Result.Latitude;
                                    _positionLongitude = t.Result.Longitude;
                                    _firstRun = false;
                                    return;
                                }

                                var testDistance = Math.Round(GpsCalculator.Distance(_positionLatitude, _positionLongitude, t.Result.Latitude, t.Result.Longitude, 'K'), 3);

                                // Check after resuming.  
                                if (_doTestCheck)
                                {
                                    // If the user moved more than 200 meters. Resuming is not allowed.
                                    if (testDistance > 0.2)
                                    {
                                        _trackChanges = false;
                                        _pause = !_pause;
                                        _locator.StopListening();
                                        PauseDistanceTooBig(testDistance);
                                        return;
                                    }
                                    _doTestCheck = false;
                                }
                            
                                // Discard if distance is below 10 meters
                                if (t.Result.Accuracy != null)
                                {
                                    SetAccuracy(t.Result.Accuracy);
                                    if (testDistance < t.Result.Accuracy/1000)
                                        return;
                                }
                                else
                                {
                                    // default accuracy
                                    if (testDistance < 0.01)
                                        return;
                                }                                

                                SetAccuracy(t.Result.Accuracy);
                                TraveledDistance = testDistance;
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
            _locator.PositionError += PositionError;
        }


        /// <summary>
        /// Method that gets called when an error event is triggered in the locator
        /// </summary>
        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            GpsNotAvailable();
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

                _countDown = Definitions.GpsCountDown; // reset timer
                PositionStatus = DateTime.Now.ToString("HH:mm:ss");
                _trackChanges = true;
                return false; //not continue
            });
        }

        #region helper methods

        /// <summary>
        /// Method to help with setting the correct text for Accuracy
        /// </summary>
        public void SetAccuracy(double? dist = null)
        {
            //if (_doCountdown)
            //{
            //    Accuracy = "Intet GPS signal"; 
            //}
            if (!_pause)
            {
                Accuracy = "GPS sat på pause";
            }
            else
            {
                if (dist != null)
                {
                    _lastAccuracy = dist.ToString();
                }
                
                Accuracy = "GPS Nøjagtighed: " + _lastAccuracy + " m";
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
                //_locator.StartListening(Definitions.MinInterval, Definitions.Accuracy, false);
                _isListening = true;
                return;
            }
            
            _pause = !_pause;
            if (_pause)
            {
                SetAccuracy();
                var test = _locator.IsGeolocationAvailable;
                if (!test)
                {
                    GpsNotAvailable();
                }
                else
                {
                    if(!_firstRun)
                        _doTestCheck = true;

                    CountDown();
                    _locator.StartListening(Definitions.MinInterval, Definitions.Accuracy, false);
                }
            }
            else
            {
                SetAccuracy();
                _locator.StopListening();
                _trackChanges = false;
                Signal = false;
                _pauseLatitude = _positionLatitude;
                _pauseLongitude = _positionLongitude;
                if (Definitions.Route.GPSCoordinates.Count > 0)
                {
                    Definitions.Route.GPSCoordinates.Last().IsViaPoint = "true";
                   
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
                    _isListening = false;
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
            if (_noGpsForTooLong)
            {
                _noGpsForTooLong = false;
                _parentPage.HandleNoGpsError();
                return;
            }

            Definitions.Report.EmploymentId = Definitions.Organization.Id;
            Definitions.Report.Date = Definitions.Date;

            Definitions.Report.ProfileId = Definitions.User.Profile.Id;
            Definitions.Report.Profile = Definitions.User.Profile;
            Definitions.Report.Route = Definitions.Route;
            Definitions.Report.Route.TotalDistance = TraveledDistance;

            Definitions.Report.StartsAtHome = Definitions.StartAtHome;
            Definitions.Report.EndsAtHome = Definitions.EndsAtHome;

            Definitions.Report.Rate = Definitions.Taxe;
            Definitions.Report.RateId = Definitions.Taxe.Id;

            if(Definitions.Report.Route.GPSCoordinates.Count == 1)
                Definitions.Report.Route.GPSCoordinates.Clear();

            Dispose();
            Navigation.PushAsync<FinishDriveViewModel>();
        }

        /// <summary>
        /// Method that handles the Appeared message
        /// </summary>
        public void HandleAppearedMessage(object sender)
        {
            if (_error && !_errorCountdownStarted && _pause)
            {
                var seconds = (DateTime.Now - _errorDateTime).TotalSeconds;
                _noSignalCountDown = (double)Definitions.NoGpsSignalTimer - seconds;
            }
            //else
            //{
            //    TestForAvailibility();
            //}
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

        public const string SignalProperty = "Signal";
        public bool Signal
        {
            get
            {
                return _signal;
            }
            set
            {
                _signal = value;
                OnPropertyChanged(SignalProperty);
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

        /// <summary>
        /// Method that checks for gps signal
        /// </summary>
        public void TestForAvailibility()
        {
            IsBusy = true;
            if(_locator != null)
            {
                _locator.GetPositionAsync(timeout: 3500, cancelToken: this._cancelSource.Token, includeHeading: false)
                    .ContinueWith(t =>
                    {
                        IsBusy = false;
                        if (t.IsFaulted)
                        {
                            GpsNotAvailable();
                            //PositionStatus = ((GeolocationException)t.Exception.InnerException).Error.ToString();
                        }
                        else if (t.IsCanceled)
                            PositionStatus = "Canceled";
                        else
                        {
                            if (t.Result.Accuracy < 100)
                            {
                                GpsIsAvailable();
                            }
                            else
                            {
                                GpsNotAvailable();
                                return;
                            }
                        }

                    });
            }
        }
    }
}
