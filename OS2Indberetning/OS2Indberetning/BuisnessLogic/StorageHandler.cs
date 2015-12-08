using Xamarin.Forms;
using XLabs.Platform.Services;

namespace OS2Indberetning.BuisnessLogic
{
 
    public static class StorageHandler
    {
        private static ISecureStorage _storage = DependencyService.Get<ISecureStorage>();
    }
}
