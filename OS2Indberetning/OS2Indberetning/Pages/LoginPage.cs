using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Services;

namespace OS2Indberetning.Pages
{
    public class LoginPage : ContentPage
    {
        private ISecureStorage storage;
        private readonly string key = "login_data";

        private LoginViewModel viewModel;
        private ListView list;

        public LoginPage()
        {
            ApiCallerMethod();
            storage = DependencyService.Get<ISecureStorage>();
            this.Content = SetTempContent();
        }

        private void ApiCallerMethod()
        {
            var objectList = APICaller.GetMunicipalityList().ContinueWith((result) =>
            {
                SetContent(result.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private View SetTempContent()
        {
            Label ne = new Label
            {
                Text = "Waiting",
            };
            var layout = new StackLayout
            {
                Children =
                {
                    ne
                }
            };

            return layout;
        }

        private void SetContent(List<Municipality> objectList )
        {
            var stringList = new List<string>();
            foreach (var item in objectList)
            {
                stringList.Add(item.Name);
            }

            list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(MunCell)),
                ItemsSource = InitList(objectList),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };

            var header = new Label
            {
                Text = "OS2Indberetning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    header,
                }
            };

            list.ItemSelected += async (sender, e) =>
            {
                var selectedItem = (MunCellModel)e.SelectedItem;
                if (selectedItem == null) return;

                foreach (var item in objectList)
                {
                    if (item.Name == selectedItem.Name)
                    {
                        await App.Navigation.PushAsync(
                                (ContentPage)
                                    ViewFactory.CreatePage<CouplingViewModel, CouplingPage>((v, vm) =>
                                    {
                                        v.InitVm(item);
                                        vm.SetMunicipality(item);
                                    } ));
                        break;
                    }
                }
                // resetter selected property så alt kan selected hvis man popper page
                list.ClearValue(ListView.SelectedItemProperty);
            };

            var layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    headerstack,
                    list
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };
            
           
            this.Content = layout;
        }

        private List<MunCellModel> InitList(List<Municipality> list)
        {
            List<MunCellModel> cellList = new List<MunCellModel>();

            foreach (var item in list)
            {
                cellList.Add(new MunCellModel
                {
                    ImageSource = new UriImageSource { Uri = new Uri(item.ImgUrl) },
                    Name = item.Name
                });
            }

            return cellList;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
