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
    /// Viewmodel of the Tax page. Handles all view logic
    /// </summary>
    public class TaxViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<GenericCellModel> _taxes;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public TaxViewModel()
        {
            _taxes = new ObservableCollection<GenericCellModel>();
            TaxList = _taxes;
            InitializeCollection();
            Subscribe();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
            _taxes = null;
            TaxList = _taxes;
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        private void Subscribe()
        {
            MessagingCenter.Subscribe<TaxPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<TaxPage, string>(this, "Selected", (sender, arg) => { HandleSelectedMessage(arg); });
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<TaxPage>(this, "Back");
            MessagingCenter.Unsubscribe<TaxPage, string>(this, "Selected");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            _taxes.Clear();

            foreach (var rate in Definitions.User.Rates)
            {
                if (Definitions.Rate != null)
                {
                    if (rate.Description == Definitions.Rate.Description)
                    {
                        _taxes.Add(new GenericCellModel { Title = rate.Description, Selected = true });
                        continue;
                    }
                }

                _taxes.Add(new GenericCellModel { Title = rate.Description, Selected = false });
                
            }
            TaxList = _taxes;
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles the Selected message
        /// </summary>
        private void HandleSelectedMessage(string arg)
        {
            foreach (var item in _taxes)
            {
                if (item.Title == arg)
                {
                    Definitions.Rate = Definitions.User.Rates.FirstOrDefault(x => x.Description == arg);
                    Definitions.Report.RateId = Definitions.User.Rates.FirstOrDefault(x => x.Description == arg).Id;
                    var json = JsonConvert.SerializeObject(Definitions.Rate);
                    FileHandler.WriteFileContent(Definitions.TaxeFileName, Definitions.TaxeFolder, json);
                    continue;
                }
                item.Selected = false;
            }
            TaxList = _taxes;
            HandleBackMessage();
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
        public const string TaxListProperty = "TaxList";
        public ObservableCollection<GenericCellModel> TaxList
        {
            get
            {
                return _taxes;
            }
            set
            {
                _taxes = value;
                OnPropertyChanged(TaxListProperty);
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
