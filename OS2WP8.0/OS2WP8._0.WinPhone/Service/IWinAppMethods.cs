/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Windows;
using OS2Indberetning.PlatformInterfaces;

[assembly: Xamarin.Forms.Dependency(typeof(IPlatformMethods))]


namespace OS2Indberetning.WinPhone.Service
{
    public class IWinAppMethods : IPlatformMethods
    {
        public void TerminateApp()
        {
            Application.Current.Terminate();
        }
    }
}
