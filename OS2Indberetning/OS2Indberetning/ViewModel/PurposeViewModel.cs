using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    public class PurposeViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Command addPurposeCommand;
        private Command backCommand;
        private Command showFieldCommand;
        private Command selectionChangedCommand;

        private string purposeAddString = null;
        private bool hideField = false;

        public ObservableCollection<PurposeString> purposes = new ObservableCollection<PurposeString>();

        public PurposeViewModel()
        {
            Subscribe();

            FileHandler.ReadFileContent(Definitions.PurposeFileName, Definitions.PurposeFolderName).ContinueWith((result) =>
            {
                if (result.Result == null) return;
                if (result.Result == "") return;
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

        public void Dispose()
        {
            Unsubscribe();
            purposes = null;
        }

        public void Subscribe()
        {
            MessagingCenter.Subscribe<PurposePage>(this, "Add", (sender) => { HideField = !HideField; });

            MessagingCenter.Subscribe<PurposePage>(this, "Back", (sender) => { HandleBackMessage(); });

            MessagingCenter.Subscribe<PurposePage>(this, "Selected", HandleSelectedMessage);
        }

        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<PurposePage>(this, "Add");

            MessagingCenter.Unsubscribe<PurposePage>(this, "Back");

            MessagingCenter.Unsubscribe<PurposePage>(this, "Selected");
        }

        #region MessageHandlers
        private void HandleBackMessage()
        {
            Dispose();
            Navigation.PopAsync();
        }

        private void HandleSelectedMessage(PurposePage sender)
        {
            var from = (PurposeString)sender.selected;
            foreach (var item in purposes)
            {
                if (item.Name == from.Name)
                {
                    Definitions.Purpose = item.Name;
                    Definitions.Report.Purpose = item.Name;
                    continue;
                }
                item.Selected = false;
            }
            PurposeList = purposes;
        }
        #endregion

        #region Properties
        public const string AddPurposeCommand = "AddPurpose";
        public ICommand AddPurpose 
        {
            get
            {
                return addPurposeCommand ?? (addPurposeCommand = new Command(() =>
                {
                    if (purposeAddString != null)
                    {
                        // Check if item already exists
                        if (purposes.FirstOrDefault(x => x.Name == purposeAddString) != null)
                        {
                            PurposeAddString = null;
                            return;
                        }
                        // Add new item
                        purposes.Add(new PurposeString{Name = purposeAddString, Selected = false});
                        // Reset field
                        PurposeAddString = null;
                        // Save list
                        FileHandler.WriteFileContent(Definitions.PurposeFileName, Definitions.PurposeFolderName, JsonConvert.SerializeObject(purposes));
                    }
                }));
            }
        }

        public const string ShowFieldProperty = "ShowField";
        public ICommand ShowField
        {
            get
            {
                return showFieldCommand ?? (showFieldCommand = new Command(() =>
                {
                    HideField = !HideField;
                }));
            }
        }

        public const string PurposeListProperty = "PurposeList";
        public ObservableCollection<PurposeString> PurposeList
        {
            get
            {
                return purposes;
            }
            set
            {
                purposes = value;
                OnPropertyChanged(PurposeListProperty);
            }
        }

        public const string PurposeStringProperty = "PurposeAddString";
        public string PurposeAddString
        {
            get
            {
                return purposeAddString;
            }
            set
            {
                purposeAddString = value;
                OnPropertyChanged(PurposeStringProperty);
            }
        }

        public const string HideFieldProperty = "HideField";
        public bool HideField
        {
            get
            {
                return hideField;
            }
            set
            {
                hideField = value;
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
}
