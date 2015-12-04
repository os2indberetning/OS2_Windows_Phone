using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
