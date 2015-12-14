using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    public class OrganizationViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<OrganizationString> organizations = new ObservableCollection<OrganizationString>();

        public OrganizationViewModel()
        {
            foreach (var employment in Definitions.User.Profile.Employments)
            {
                if (employment.Id == Definitions.Report.EmploymentId)
                {
                    organizations.Add(new OrganizationString { Name = employment.EmploymentPosition, Selected = true });
                    continue;
                }
                organizations.Add(new OrganizationString{Name = employment.EmploymentPosition, Selected = false});
            }
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
            organizations = null;
        }

        public void Subscribe()
        {
            MessagingCenter.Subscribe<OrganizationPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<OrganizationPage, string>(this, "Selected", (sender, arg) => { HandleSelectedMessage(arg); });
        }

        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<OrganizationPage>(this, "Back");
            MessagingCenter.Unsubscribe<OrganizationPage, string>(this, "Selected");
        }

        #region Message Handlers
        private void HandleSelectedMessage(string arg)
        {
            foreach (var item in organizations)
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
            OrganizationList = organizations;
        }
        private void HandleBackMessage()
        {
            Dispose();
            Navigation.PopToRootAsync();
        }
        #endregion

        #region Properties
        public const string OrganizationListProperty = "OrganizationList";
        public ObservableCollection<OrganizationString> OrganizationList
        {
            get
            {
                return organizations;
            }
            set
            {
                organizations = value;
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
