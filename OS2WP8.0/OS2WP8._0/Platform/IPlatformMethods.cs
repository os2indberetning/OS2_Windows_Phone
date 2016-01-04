namespace OS2Indberetning.PlatformInterfaces
{

    /// <summary>
    /// Interface to platform specific methods, implemented in their respective projects.
    /// </summary>
    public interface IPlatformMethods
    {

        /// <summary>
        /// Method that terminates the application
        /// </summary>
        void TerminateApp();
    }
}
