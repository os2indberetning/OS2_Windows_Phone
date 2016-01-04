﻿using System;
using OS2Indberetning.Model;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace OS2Indberetning.Pages
{
    /// <summary>
    /// Page that is displayed when trying to couple your phone with a Municipality
    /// </summary>
    public class CouplingPage : ContentPage
    {
        private Municipality _municipality;

        private int hackSpaces;
        private string _placeholder = "";
        public CouplingPage()
        {
            // HACK to center placeholder text. best method i could find
            hackSpaces = (int)Math.Round(Definitions.ScreenWidth/27);
            for (int i = 0; i < hackSpaces; i++)
            {
                _placeholder = _placeholder + " ";
            }
            _placeholder = _placeholder + "Tast Parringskode";
        }

        /// <summary>
        /// Method is used as a constructor. The real constructor needs to be parameterless
        /// </summary>
        /// <param name="m">Municipality that the user is trying to couple with</param>
        public void SetMunicipality(Municipality m)
        {
            _municipality = m;
            Definitions.TextColor = _municipality.TextColor;
            Definitions.PrimaryColor = _municipality.PrimaryColor;
            Definitions.SecondaryColor = _municipality.SecondaryColor;
            Definitions.MunIcon = new UriImageSource {Uri = new Uri(m.ImgUrl)};
            Definitions.MunUrl = _municipality.APIUrl;
            this.Content = SetContent();
        }


        /// <summary>
        /// Method used to create the layout of the page
        /// </summary>
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

            var entry = new Entry
            {
                Placeholder = _placeholder,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.FromHex(Definitions.DefaultTextColor),
            };
            entry.SetBinding(Entry.TextProperty, CouplingViewModel.TokenProperty);

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

            var coupleButton = new ButtomButton("Par Telefon", SendCoupleMessage);
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

        /// <summary>
        /// Method that sends a Couple message
        /// </summary>
        private void SendCoupleMessage()
        {
            MessagingCenter.Send<CouplingPage>(this, "Couple");
        }

        /// <summary>
        /// Method that sends a Back message
        /// </summary>
        private void SendBackMessage()
        {
            MessagingCenter.Send<CouplingPage>(this, "Back");
        }

        /// <summary>
        /// Method that overrides the BackbuttonPressed event. 
        /// Calls SendBackMessage so that the logic is handles by the viewmodel
        /// </summary>
        /// <returns>true so that the event is "overlooked"</returns>
        protected override bool OnBackButtonPressed()
        {
            SendBackMessage();
            return true;
        }
    }


}