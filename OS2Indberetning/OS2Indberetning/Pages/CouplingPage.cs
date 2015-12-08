using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRenderer;
using OS2Indberetning.Model;
using OS2Indberetning.PlatformInterfaces;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Pages
{
    public class CouplingPage : ContentPage
    {
        private Municipality municipality;
        private CouplingViewModel viewModel;

        public CouplingPage()
        {
        }

        public void SetMunicipality(Municipality m)
        {
            municipality = m;
            Definitions.TextColor = municipality.TextColor;
            Definitions.PrimaryColor = municipality.PrimaryColor;
            Definitions.SecondaryColor = municipality.SecondaryColor;
            Definitions.MunIcon = new UriImageSource {Uri = new Uri(m.ImgUrl)};
            Definitions.MunUrl = municipality.APIUrl;
            this.Content = SetContent();
        }



        private View SetContent()
        {

            var header = new Label
            {
                Text = "Parring",
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

            var entry = new ExtendedEntry()
            {
                Placeholder = "Tast Parringskode",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                HasBorder = true,
            };
            entry.SetBinding(ExtendedEntry.TextProperty, CouplingViewModel.TokenProperty);

            var informationText = new Label
            {
                Text = "For at bruge appen skal du parre din telefon med din bruger på OS2Indberetning.\n" +
                       "\n" +
                       "For at parre telefonen skal du bruge et token\n" +
                       "\n" +
                       "Et token er et unikt nummer der forbinder din bruger på OS2Indberetning med din telefon.\n" +
                       "\n" +
                       "Du laver et token ved at gå ind under \"Personlige Indstillinger\" på hjemmesiden og trykke \"Mine Tokens\".\n" +
                       "\n" +
                       "Herefter kan du se dit token, som du bare skal indtaste i feltet ovenfor, og derefter trykke på \"Par Telefon\"",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
                FontSize = Definitions.InformationFontSize,
            };

            var textFrame = new StackLayout
            {
                Padding = Definitions.Padding,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children =
                {
                    informationText
                }
            };

            var coupleButton = new ButtomButton("Par Telefon", Couple);
            var buttomStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.End,
                Padding = Definitions.Padding,
                HeightRequest = Definitions.ButtonHeight,

                Children = { coupleButton }
            };

            var layout = new StackLayout
            {
                Children =
                {
                    headerstack,
                    entry,
                    textFrame,
                    buttomStack,
                },
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };

            return layout;
        }

        private void Couple()
        {
            MessagingCenter.Send<CouplingPage>(this, "Couple");
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    if (Device.OS == TargetPlatform.WinPhone)
        //    {
        //        Navigation.PopAsync(true);
        //    }
        //    return true;
        //}
    }


}
