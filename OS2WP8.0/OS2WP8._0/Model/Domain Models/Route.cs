/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Collections.Generic;

namespace OS2Indberetning.Model
{
    public class Route
    {
        public Route()
        {
            GPSCoordinates = new List<GPSCoordinate>();
        }
        public int Id { get; set; }
        public int DriveReportId { get; set; }
        public double TotalDistance { get; set; }
        public List<GPSCoordinate> GPSCoordinates { get; set; } 
    }
}
