using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS2Indberetning.Model;
using Xamarin.Forms;
using XLabs.Platform.Services;

namespace OS2Indberetning.BuisnessLogic
{
 
    public static class StorageHandler
    {
        private static ISecureStorage _storage = DependencyService.Get<ISecureStorage>();
    }
}
