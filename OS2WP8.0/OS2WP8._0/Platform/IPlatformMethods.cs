/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
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
