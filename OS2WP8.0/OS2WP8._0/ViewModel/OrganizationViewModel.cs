/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using OS2WP8._0.Model.TemplateModels;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Organization page. Handles all view logic
    /// </summary>
    public class OrganizationViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<GenericCellModel> _organizations;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public OrganizationViewModel()
        {
            _organizations = new ObservableCollection<GenericCellModel>();
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
                if (Definitions.Organization != null)
                {
                    if (employment.Id == Definitions.Organization.Id)
                    {
                        _organizations.Add(new GenericCellModel { Title = employment.EmploymentPosition, SubTitle = employment.ManNr ,  Selected = true });
                        continue;
                    }
                }

                _organizations.Add(new GenericCellModel { Title = employment.EmploymentPosition, SubTitle = employment.ManNr, Selected = false });
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
                if (item.Title == arg)
                {
                    Definitions.Organization =
                        Definitions.User.Profile.Employments.FirstOrDefault(x => x.EmploymentPosition == arg);
                    Definitions.Report.EmploymentId = Definitions.Organization.Id;
                    var json = JsonConvert.SerializeObject(Definitions.Organization);
                    FileHandler.WriteFileContent(Definitions.OrganizationFileName, Definitions.OrganizationFolder, json).ContinueWith(
                        result =>
                        {
                            HandleBackMessage(); // Solves the problem where the list gets disposed before the view has exited the screen
                        }, TaskScheduler.FromCurrentSynchronizationContext());
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
        public ObservableCollection<GenericCellModel> OrganizationList
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
}
