/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;

using OS2Indberetning.Model;


namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel that handles the view logic of FinishDrivePage
    /// </summary>
    public class FinishDriveViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _startHomeCheck;
        private bool _endHomeCheck;
        private bool _fourKmRuleCheck;
        private bool _showFourKmRule;
        private string _date;
        private string _username;
        private string _newKm;
        private string _homeToBorderDistance;
        private string _purpose;
        private string _organization;
        private string _rate;
        private string _remark;

        private string _homeToBorderDistanceKey = Definitions.HomeToBorderDistanceKey + Definitions.User.Profile.Authorization.GuId;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public FinishDriveViewModel()
        {
            bool containKey = Application.Current.Properties.ContainsKey(_homeToBorderDistanceKey);
            double d = 0;
            if (containKey)
            {
                d = (double)Application.Current.Properties[_homeToBorderDistanceKey];
            }
            Definitions.Report.HomeToBorderDistance = d;
            NewKm = Definitions.Report.Route.TotalDistance.ToString();

            InitializeCollection();

            Username = Definitions.User.Profile.FirstName + " " + Definitions.User.Profile.LastName;
            Date = Definitions.DateToView;

            Subscribe();
        }

        /// <summary>
        /// Destructor 
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Method that subscribes to nessecary calls
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Upload", (sender) => UploadHandler());
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Delete", HandleDeleteMessage);
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Update", (sender) => { HandleUpdateMessage();});
            MessagingCenter.Subscribe<FinishDrivePage>(this, "EndHome",(sender) => { StartHomeCheck = !StartHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "StartHome", (sender) => { EndHomeCheck = !EndHomeCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "FourKmRule", (sender) => { FourKmRuleCheck = !FourKmRuleCheck; });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Purpose", (sender) => { HandlePurposeMessage(); });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Organization", (sender) => { HandleOrganizationMessage(); });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Rate", (sender) => { HandleRateMessage(); });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "Remark", (sender) => { HandleRemarkMessage(); });
            MessagingCenter.Subscribe<FinishDrivePage>(this, "SelectHomeToBorderDistance", HandleSelectHomeToBorderDistanceMessage);
            MessagingCenter.Subscribe<FinishDrivePage>(this, "HomeToBorderDistance", HandleHomeToBorderDistanceMessage);
        }

        /// <summary>
        /// Method that handles all unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Upload");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Delete");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Update");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "EndHome");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "StartHome");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "FourKmRule");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Purpose");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Organization");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Rate");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "Remark");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "SelectHomeToBorderDistance");
            MessagingCenter.Unsubscribe<FinishDrivePage>(this, "HomeToBorderDistance");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            if (String.IsNullOrEmpty(Definitions.Report.ManualEntryRemark))
            {
                Definitions.Report.ManualEntryRemark = "Indtast eventuelle uddybende kommentarer";
            }

            Purpose = Definitions.Report.Purpose;
            Organization = Definitions.User.Profile.Employments.FirstOrDefault(x => x.Id == Definitions.Report.EmploymentId).EmploymentPosition;
            Rate = Definitions.User.Rates.FirstOrDefault(x => x.Id == Definitions.Report.RateId).Description;
            Remark = Definitions.Report.ManualEntryRemark;
            NewKm = Convert.ToString(Math.Round(Definitions.Report.Route.TotalDistance, 1));
            HomeToBorderDistance = Convert.ToString(Definitions.Report.HomeToBorderDistance);
            StartHomeCheck = Definitions.Report.StartsAtHome;
            EndHomeCheck = Definitions.Report.EndsAtHome;
            FourKmRuleCheck = Definitions.Report.FourKmRule;
            ShowFourKmRule = Definitions.User.Profile.Employments.FirstOrDefault(x => x.Id == Definitions.Report.EmploymentId).OrgUnit.FourKmRuleAllowed;

        }

        #region Message Handlers

        /// <summary>
        /// Method that handles a select hometoborderdistance message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandleSelectHomeToBorderDistanceMessage(FinishDrivePage sender)
        {
            sender.PopUpLayout.ShowPopup(sender.HomeToBorderDistancePopup());
        }

        /// <summary>
        /// Method that handles a Update message from the page
        /// </summary>
        private void HandleUpdateMessage()
        {
            InitializeCollection();
        }

        /// <summary>
        /// Method that handles a Purpose message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandlePurposeMessage()
        {
            Navigation.PushModalAsync<PurposeViewModel>();
        }

        /// <summary>
        /// Method that handles a Organization message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandleOrganizationMessage()
        {
            Navigation.PushModalAsync<OrganizationViewModel>();
        }

        /// <summary>
        /// Method that handles a Rate message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandleRateMessage()
        {
            Navigation.PushModalAsync<TaxViewModel>();
        }

        /// <summary>
        /// Method that handles a Remark message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandleRemarkMessage()
        {
            Navigation.PushModalAsync<RemarkViewModel>();
        }

        /// <summary>
        /// Method that handles a HomeToBorderDistance message from the page
        /// </summary>
        /// <param name="sender"></param>
        private void HandleHomeToBorderDistanceMessage(FinishDrivePage sender)
        {
            try
            {
                Definitions.Report.HomeToBorderDistance = Convert.ToDouble(_homeToBorderDistance);
            }
            catch (Exception e)
            {
                // ONLY happens if user somehow writes letters with numeric keyboard?  
                // Can happen in a simulator
            }
            InitializeCollection();
            sender.PopUpLayout.DismissPopup();
        }

        /// <summary>
        /// Method that handles a Back message from the page
        /// Calls dispose, sets report route to null and navigates to mainpage
        /// </summary>
        private void HandleDeleteMessage(object sender)
        {
            Definitions.Report.Route = null;
            Definitions.Purpose = null;
            Dispose();
            App.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Method that handles a Upload message from the page
        /// Calls dispose and navigates to uploadingpage
        /// </summary>
        private void UploadHandler()
        {
            Dispose();
            Navigation.PushAsync<UploadingViewModel>();
        }

        #endregion

        #region properties
  
        public const string StartHomeCheckProperty = "StartHomeCheck";
        public bool StartHomeCheck
        {
            get
            {
                return _startHomeCheck;
            }
            set
            {
                _startHomeCheck = value;
                Definitions.Report.StartsAtHome = value;
                OnPropertyChanged(StartHomeCheckProperty);
            }
        }

        public const string EndHomeCheckProperty = "EndHomeCheck";
        public bool EndHomeCheck
        {
            get
            {
                return _endHomeCheck;
            }
            set
            {
                _endHomeCheck = value;
                Definitions.Report.EndsAtHome = value;
                OnPropertyChanged(EndHomeCheckProperty);
            }
        }

        public const string ShowFourKmRuleProperty = "ShowFourKmRule";
        public bool ShowFourKmRule
        {
            get
            {
                return _showFourKmRule;
            }
            set
            {
                _showFourKmRule = value;
                if (!_showFourKmRule)
                {
                    FourKmRuleCheck = false;
                }
                OnPropertyChanged(ShowFourKmRuleProperty);
            }
        }

        public const string FourKmRuleCheckProperty = "FourKmRuleCheck";
        public bool FourKmRuleCheck
        {
            get
            {
                return _fourKmRuleCheck;
            }
            set
            {
                _fourKmRuleCheck = value;
                Definitions.Report.FourKmRule = value;
                OnPropertyChanged(FourKmRuleCheckProperty);
            }
        }

        public const string HomeToBorderDistanceProperty = "HomeToBorderDistance";
        public string HomeToBorderDistance
        {
            get
            {
                if (Application.Current.Properties.ContainsKey(_homeToBorderDistanceKey))
                {
                    string distance = Convert.ToString((double)Application.Current.Properties[_homeToBorderDistanceKey]);
                    if (distance != null)
                    {
                        if (distance != "0")
                        {
                            return distance;
                        }
                    }
                }
                
                return _homeToBorderDistance;
            }
            set
            {
                if (_homeToBorderDistance == value)
                {
                    return;
                }
                
                _homeToBorderDistance = value;
               
                Application.Current.Properties[_homeToBorderDistanceKey] = String.IsNullOrEmpty(_homeToBorderDistance) ? 0 : Convert.ToDouble(_homeToBorderDistance);
                OnPropertyChanged(HomeToBorderDistanceProperty);
            }
        }


        public const string NewKmProperty = "NewKm";
        public string NewKm
        {
            get { return _newKm; }
            set
            {
                _newKm = value;
                OnPropertyChanged(NewKmProperty);
            }
        }

        public const string DateProperty = "Date";
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(DateProperty);
            }
        }

        public const string UsernameProperty = "Username";
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(UsernameProperty);
            }
        }

        public const string PurposeProperty = "Purpose";
        public string Purpose
        {
            get
            {
                return _purpose;
            }
            set
            {
                if(_purpose == value)
                {
                    return;
                }
                _purpose = value;
                OnPropertyChanged(PurposeProperty);
            }
        }

        public const string OrganizationProperty = "Organization";
        public string Organization
        {
            get
            {
                return _organization;
            }
            set
            {
                if (_organization == value)
                {
                    return;
                }
                _organization = value;
                OnPropertyChanged(OrganizationProperty);
            }
        }

        public const string RateProperty = "Rate";
        public string Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                if (_rate == value)
                {
                    return;
                }
                _rate = value;
                OnPropertyChanged(RateProperty);
            }
        }

        public const string RemarkProperty = "Remark";
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                if (_remark == value)
                {
                    return;
                }
                _remark = value;
                OnPropertyChanged(RemarkProperty);
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
