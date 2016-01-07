﻿/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
namespace OS2Indberetning.Model
{
    public class DriveReport
    {
        public DriveReport()
        {
            Profile = new Profile();
            Rate = new Rate();
        }
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int ProfileId { get; set; }
        public int RateId { get; set; }
        public int EmploymentId { get; set; }
        public string Date  { get; set; }
        public string ManualEntryRemark { get; set; }
        public string Purpose { get; set; }
        public bool StartsAtHome { get; set; }
        public bool EndsAtHome { get; set; }
        public Route Route { get; set; }
        public Rate Rate { get; set; }
        public Profile Profile { get; set; }
    }
}
