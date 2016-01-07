/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
namespace OS2Indberetning.Model
{
    public class GPSCoordinate
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string IsViaPoint = "false";
    }
}