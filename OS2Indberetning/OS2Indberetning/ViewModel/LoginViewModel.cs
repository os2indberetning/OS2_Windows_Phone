using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Pages;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel to the Login page. Setups the list for display in the page
    /// and handles pushing to the couple page with the correct model
    /// </summary>
    public class LoginViewModel : XLabs.Forms.Mvvm.ViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<MunCellModel> _munList;
        private List<Municipality> _objectList;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public LoginViewModel()
        {
            MunList = new ObservableCollection<MunCellModel>();
            APICaller.GetMunicipalityList().ContinueWith((result) =>
            {
                _objectList = result.Result;
                InitList(result.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Method that initializes the Municipality list
        /// </summary>
        private void InitList(List<Municipality> resultList)
        {
            _munList.Clear();

            foreach (var item in resultList)
            {
                _munList.Add(new MunCellModel
                {
                    ImageSource = new UriImageSource { Uri = new Uri(item.ImgUrl) },
                    Name = item.Name
                });
            }

            MunList = _munList;
        }

        /// <summary>
        /// Method called from the page when an item is Selected.
        /// It pushes a coupling page initialized to the Selected Municipality.
        /// Used instead of messagingcenter
        /// </summary>
        public void OnSelectedItem(MunCellModel selectedItem)
        {
            foreach (var item in _objectList)
            {
                if (item.Name == selectedItem.Name)
                {
                    App.Navigation.PushAsync(
                            (ContentPage)
                                ViewFactory.CreatePage<CouplingViewModel, CouplingPage>((v, vm) =>
                                {
                                    v.InitVm(item);
                                    vm.SetMunicipality(item);
                                }));
                    break;
                }
            }
        }

        #region Properties

        public const string MunListProperty = "MunList";
        public ObservableCollection<MunCellModel> MunList
        {
            get { return _munList; }
            set
            {
                _munList = value;
                OnPropertyChanged(MunListProperty);
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
