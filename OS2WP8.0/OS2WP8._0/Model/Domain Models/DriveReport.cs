/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
namespace OS2Indberetning.Model
{
    public class DriveReport
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string Uuid { get; set; }
        public int ProfileId { get; set; }
        public int RateId { get; set; }
        public int EmploymentId { get; set; }
        public string Date  { get; set; }
        public string ManualEntryRemark { get; set; }
        public string Purpose { get; set; }
        public bool StartsAtHome { get; set; }
        public bool EndsAtHome { get; set; }
        public Route route { get; set; }
        public bool FourKmRule { get; set; }
        public double HomeToBorderDistance { get; set; }
    }
}
