/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
namespace OS2Indberetning.Model
{
    /// <summary>
    /// Class to wrap DriveReport and Token
    /// </summary>
    public class DriveSubmit
    {
        public DriveReport DriveReport { get; set; }
        public Token Token { get; set; }
    }
}
