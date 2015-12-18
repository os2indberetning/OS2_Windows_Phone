﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Organization page. Handles all view logic
    /// </summary>
    public class OrganizationViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<OrganizationString> _organizations;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public OrganizationViewModel()
        {
            _organizations = new ObservableCollection<OrganizationString>();
            InitializeCollection();
            Subscribe();
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
            _organizations = null;
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        public void Subscribe()
        {
            MessagingCenter.Subscribe<OrganizationPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<OrganizationPage, string>(this, "Selected", (sender, arg) => { HandleSelectedMessage(arg); });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<OrganizationPage>(this, "Back");
            MessagingCenter.Unsubscribe<OrganizationPage, string>(this, "Selected");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            foreach (var employment in Definitions.User.Profile.Employments)
            {
                if (employment.Id == Definitions.Report.EmploymentId)
                {
                    _organizations.Add(new OrganizationString { Name = employment.EmploymentPosition, Selected = true });
                    continue;
                }
                _organizations.Add(new OrganizationString { Name = employment.EmploymentPosition, Selected = false });
            }
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles the Selected message
        /// </summary>
        private void HandleSelectedMessage(string arg)
        {
            foreach (var item in _organizations)
            {
                if (item.Name == arg)
                {
                    Definitions.Organization =
                        Definitions.User.Profile.Employments.FirstOrDefault(x => x.EmploymentPosition == arg);
                    Definitions.Report.EmploymentId = Definitions.Organization.Id;
                    continue;
                }
                item.Selected = false;
            }
            OrganizationList = _organizations;
        }

        /// <summary>
        /// Method that handles the Back message
        /// </summary>
        private void HandleBackMessage()
        {
            Dispose();
            Navigation.PopModalAsync();
        }
        #endregion

        #region Properties
        public const string OrganizationListProperty = "OrganizationList";
        public ObservableCollection<OrganizationString> OrganizationList
        {
            get
            {
                return _organizations;
            }
            set
            {
                _organizations = value;
                OnPropertyChanged(OrganizationListProperty);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    
    // List template model
    #region OrganizationString
    public class OrganizationString : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; }
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            } 
        }
    
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
