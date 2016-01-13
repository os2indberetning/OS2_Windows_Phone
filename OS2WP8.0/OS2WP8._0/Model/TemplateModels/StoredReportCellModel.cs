/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
namespace OS2Indberetning.Model
{
    public class StoredReportCellModel
    {
        public string Date { get; set; }
        public string Distance { get; set; }
        public string Purpose { get; set; }
        public string Taxe { get; set; }
        public DriveReport report { get; set; }
    }
}
