﻿/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Collections.Generic;

namespace OS2Indberetning.Model
{
    public class UserInfoModel
    {
        public List<Rate> Rates { get; set; }
        public Profile Profile { get; set; }
    }
}
