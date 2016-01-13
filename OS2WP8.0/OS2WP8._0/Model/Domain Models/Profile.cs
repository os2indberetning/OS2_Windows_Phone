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
    public class Profile
    {
        public Profile()
        {
            Employments = new List<Employment>();
            Tokens = new List<Token>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomeLatitude { get; set; }
        public string HomeLongitude { get; set; }
        public List<Employment> Employments { get; set; }
        public List<Token> Tokens { get; set; } 
    }
}
