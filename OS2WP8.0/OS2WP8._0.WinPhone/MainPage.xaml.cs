using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace OS2WP8._0.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            //SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
            SupportedOrientations = SupportedPageOrientation.Portrait; // Force Portrait
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new OS2Indberetning.App());
        }
    }
}
