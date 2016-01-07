/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;

namespace OS2Indberetning.BuisnessLogic
{        
    /// <summary>
    /// Class to handle distance calculations
    /// </summary>
    public static class GpsCalculator
    {
        /// <summary>
        /// Calculates the distance between 2 sets of gps coordinates
        /// </summary>
        /// <param name="lat1">latitude of coordinate 1</param>
        /// <param name="lon1">longitude of coordinate 1</param>
        /// <param name="lat2">latitude of coordinate 2</param>
        /// <param name="lon2">longitude of coordinate 2</param>
        /// <param name="unit">char indicating what unit to be used for calculating the distance</param>
        /// <returns>distance between the 2 coordinates</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        /// <summary>
        /// Convertes degree to radians
        /// </summary>
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
    }
}
