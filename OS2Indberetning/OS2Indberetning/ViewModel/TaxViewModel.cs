using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    public class TaxViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TaxString> taxes = new ObservableCollection<TaxString>();

        public TaxViewModel()
        {
            foreach (var rate in Definitions.User.Rates)
            {
                if (rate.Description == Definitions.Report.Rate.Description)
                {
                    taxes.Add(new TaxString { Name = rate.Description, Selected = true });
                    continue;
                }
                taxes.Add(new TaxString { Name = rate.Description, Selected = false });
            }
            TaxList = taxes;
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
            taxes = null;
        }

        public void Subscribe()
        {
            MessagingCenter.Subscribe<TaxPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<TaxPage, string>(this, "Selected", (sender, arg) => { HandleSelectedMessage(arg); });
        }

        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<TaxPage>(this, "Back");
            MessagingCenter.Unsubscribe<TaxPage>(this, "Selected");
        }

        #region Message Handlers
        private void HandleSelectedMessage(string arg)
        {
            foreach (var item in taxes)
            {
                if (item.Name == arg)
                {
                    Definitions.Taxe = Definitions.User.Rates.FirstOrDefault(x => x.Description == arg);
                    Definitions.Report.Rate = Definitions.Taxe;
                    continue;
                }
                item.Selected = false;
            }
            TaxList = taxes;
        }
        private void HandleBackMessage()
        {
            Dispose();
            App.Navigation.PopToRootAsync();
        }
        #endregion

        #region Properties
        public const string TaxListProperty = "TaxList";
        public ObservableCollection<TaxString> TaxList
        {
            get
            {
                return taxes;
            }
            set
            {
                taxes = value;
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

    #region TaxString
    public class TaxString : INotifyPropertyChanged
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
