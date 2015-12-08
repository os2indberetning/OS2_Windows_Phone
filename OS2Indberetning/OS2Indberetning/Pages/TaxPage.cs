using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;

namespace OS2Indberetning.Pages
{
    public class TaxPage : ContentPage
    {
        public TaxString selected;

        public TaxPage()
        {
            SetContent();
        }

        public void SetContent()
        {
            var header = new Label
            {
                Text = "Vælg Takst",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            // filler to make header is centered
            var filler = new Filler();
            // they need to be the same size.
            backButton.WidthRequest = backButton.WidthRequest - 40;
            filler.WidthRequest = backButton.WidthRequest;

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    filler
                }
            };

            var list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GenericCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, TaxViewModel.TaxListProperty, BindingMode.TwoWay);

            list.ItemSelected += (sender, args) =>
            {
                var item = (TaxString) args.SelectedItem;

                if (item == null) return;
                item.Selected = true;
                selected = item;
                SendSelectedMessage();
            };

            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };


            layout.Children.Add(headerstack);
            layout.Children.Add(list);

            this.Content = layout;

        }

        private void SendBackMessage()
        {
            MessagingCenter.Send<TaxPage>(this, "Back");
        }

        private void SendSelectedMessage()
        {
            MessagingCenter.Send<TaxPage, string>(this, "Selected", selected.Name);
        }
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }


    }
}
