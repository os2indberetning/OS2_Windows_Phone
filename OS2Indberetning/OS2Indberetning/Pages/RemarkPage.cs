using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OS2Indberetning.BuisnessLogic;
using OS2Indberetning.Templates;
using OS2Indberetning.ViewModel;
using PCLStorage;
using Xamarin.Forms;
using XLabs.Enums;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace OS2Indberetning.Pages
{
    public class RemarkPage : ContentPage
    {
        private Editor editor;
        public RemarkPage()
        {
            SetContent();
        }

        public void SetContent()
        {
            var header = new Label
            {
                Text = "Bemærkning",
                TextColor = Color.FromHex(Definitions.TextColor),
                FontSize = Definitions.HeaderFontSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                YAlign = TextAlignment.Center,
            };
            
            var backButton = new BackButton(SendBackMessage);
            var saveButton = new SaveButton(SendSaveMessage);

            var headerstack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromHex(Definitions.PrimaryColor),
                HeightRequest = Definitions.HeaderHeight,
                Children =
                {
                    backButton,
                    header,
                    saveButton
                }
            };
            var display = Resolver.Resolve<IDevice>().Display;
            editor = new Editor
            {
                HeightRequest = 300,
                WidthRequest = display.Width - 10,
                HorizontalOptions = LayoutOptions.Center,
            };
            editor.SetBinding(Editor.TextProperty, RemarkViewModel.RemarkProperty);
            
            var layout = new StackLayout
            {
                BackgroundColor = Color.FromHex(Definitions.BackgroundColor),
            };


            layout.Children.Add(headerstack);
            layout.Children.Add(editor);

            this.Content = layout;
        }

        private void SendBackMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Back");
        }
        private void SendSaveMessage()
        {
            MessagingCenter.Send<RemarkPage>(this, "Save");
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync();
            return true;
        }
       
    }
}
