using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Pages
{
    public class PurposePage : ContentPage
    {
        private PurposeViewModel viewModel;
        public PurposeString selected;
        private string[] purposeArray;
        public PurposePage()
        {
            SetContent();
        }

        public void SetContent()
        {
            var header = new Label
            {
                Text = "Vælg Formål",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var addButton = new AddButton(SendAddMessage);
            var backButton = new BackButton(SendBackMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    addButton
                }
            };

            var list = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GenericCell)),
                SeparatorColor = Color.FromHex("#EE2D2D"),
                SeparatorVisibility = SeparatorVisibility.Default,
            };
            list.SetBinding(ListView.ItemsSourceProperty, PurposeViewModel.PurposeListProperty, BindingMode.TwoWay);

            list.ItemSelected += (sender, args) =>
            {
                var item = (PurposeString) args.SelectedItem;

                if (item == null) return;
                item.Selected = true;
                selected = item;
                SendSelectedMessage();
            };

            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            var entry = new ExtendedEntry()
            {
                Placeholder = "Tast nyt formål",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                HasBorder = true,
            };
            entry.SetBinding(ExtendedEntry.TextProperty, PurposeViewModel.PurposeStringProperty);

            var addPurpose = new Button
            {
                Text = "Tilføj",
                BorderColor = Color.FromHex(Definitions.PrimaryColor),
                TextColor = Color.FromHex(Definitions.TextColor),
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                AnchorY = (double)TextAlignment.Center,
                FontSize = 20,
                BorderRadius = 100,
            };
            addPurpose.SetBinding(Button.CommandProperty, PurposeViewModel.AddPurposeCommand);

            var addStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    entry,
                    addPurpose
                }
            };
            addStack.SetBinding(StackLayout.IsVisibleProperty, PurposeViewModel.HideFieldProperty);

            layout.Children.Add(headerstack);
            layout.Children.Add(addStack);
            layout.Children.Add(list);

            this.Content = layout;

        }

        private void SendAddMessage()
        {
            MessagingCenter.Send<PurposePage>(this, "Add");
        }

        private void SendBackMessage()
        {
            MessagingCenter.Send<PurposePage>(this, "Back");
        }

        private void SendSelectedMessage()
        {
            MessagingCenter.Send<PurposePage>(this, "Selected");

            MessagingCenter.Send<PurposePage, string>(this, "Selected", selected.Name);
        }
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }


    }
}
