/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using Windows.ApplicationModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OS2Indberetning;

namespace OS2WP8._0.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            //SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
            SupportedOrientations = SupportedPageOrientation.Portrait; // Force Portrait

            // Set the version number
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = new XmlXapResolver()
            };
            using (var xmlReader = XmlReader.Create("WMAppManifest.xml", xmlReaderSettings))
            {
                xmlReader.ReadToDescendant("App");

                Definitions.VersionNumber = xmlReader.GetAttribute("Version");
            }

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new OS2Indberetning.App());
        }
    }
}
