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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Purpose page. Handles all view logic
    /// </summary>
    public class PurposeViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Command _addPurposeCommand;

        private string _purposeAddString = null;
        private bool _hideField = false;

        private ObservableCollection<PurposeString> _purposes;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public PurposeViewModel()
        {
            _purposes = new ObservableCollection<PurposeString>();
            InitializeCollection();
            Subscribe();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
            _purposes = null;
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        public void Subscribe()
        {
            MessagingCenter.Subscribe<PurposePage>(this, "Add", (sender) => { HideField = !HideField; });

            MessagingCenter.Subscribe<PurposePage>(this, "Back", (sender) => { HandleBackMessage(); });

            MessagingCenter.Subscribe<PurposePage>(this, "Selected", HandleSelectedMessage);
        }

        /// <summary>
        /// Method that handles unsubscribing
        /// </summary>
        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<PurposePage>(this, "Add");

            MessagingCenter.Unsubscribe<PurposePage>(this, "Back");

            MessagingCenter.Unsubscribe<PurposePage>(this, "Selected");
        }

        /// <summary>
        /// Method that handles initialization of the observerable collection
        /// </summary>
        private void InitializeCollection()
        {
            FileHandler.ReadFileContent(Definitions.PurposeFileName, Definitions.PurposeFolderName).ContinueWith((result) =>
            {
                if (String.IsNullOrEmpty(result.Result))
                {
                    HideField = true;
                    return;
                }

                var temp = JsonConvert.DeserializeObject<ObservableCollection<PurposeString>>(result.Result);
                foreach (PurposeString item in temp)
                {
                    if (item.Name == Definitions.Report.Purpose)
                    {
                        item.Selected = true;
                        continue;
                    }
                    item.Selected = false;
                }
                PurposeList = temp;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles the Back message
        /// </summary>
        private void HandleBackMessage()
        {
            Dispose();
            Navigation.PopModalAsync();
        }

        /// <summary>
        /// Method that handles the Selected message
        /// </summary>
        private void HandleSelectedMessage(PurposePage sender)
        {
            var from = (PurposeString)sender.Selected;
            foreach (var item in _purposes)
            {
                if (item.Name == from.Name)
                {
                    Definitions.Purpose = item.Name;
                    Definitions.Report.Purpose = item.Name;
                    continue;
                }
                item.Selected = false;
            }
            PurposeList = _purposes;
        }

        #endregion

        #region Properties
        public const string AddPurposeCommand = "AddPurpose";
        public ICommand AddPurpose
        {
            get
            {
                return _addPurposeCommand ?? (_addPurposeCommand = new Command(() =>
                {
                    if (_purposeAddString != null)
                    {
                        // Check if item already exists
                        if (_purposes.FirstOrDefault(x => x.Name == _purposeAddString) != null)
                        {
                            PurposeAddString = null;
                            return;
                        }
                        // Add new item
                        _purposes.Add(new PurposeString { Name = _purposeAddString, Selected = false });
                        // Reset field
                        PurposeAddString = null;
                        // Save list
                        FileHandler.WriteFileContent(Definitions.PurposeFileName, Definitions.PurposeFolderName, JsonConvert.SerializeObject(_purposes));
                    }
                }));
            }
        }

        //public const string ShowFieldProperty = "ShowField";
        //public ICommand ShowField
        //{
        //    get
        //    {
        //        return showFieldCommand ?? (showFieldCommand = new Command(() =>
        //        {
        //            HideField = !HideField;
        //        }));
        //    }
        //}

        public const string PurposeListProperty = "PurposeList";
        public ObservableCollection<PurposeString> PurposeList
        {
            get
            {
                return _purposes;
            }
            set
            {
                _purposes = value;
                OnPropertyChanged(PurposeListProperty);
            }
        }

        public const string PurposeStringProperty = "PurposeAddString";
        public string PurposeAddString
        {
            get
            {
                return _purposeAddString;
            }
            set
            {
                _purposeAddString = value;
                OnPropertyChanged(PurposeStringProperty);
            }
        }

        public const string HideFieldProperty = "HideField";
        public bool HideField
        {
            get
            {
                return _hideField;
            }
            set
            {
                _hideField = value;
                OnPropertyChanged(HideFieldProperty);
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
    #region PurposeString

    public class PurposeString : INotifyPropertyChanged
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
