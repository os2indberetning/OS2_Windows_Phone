/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using Xamarin.Forms;

namespace OS2Indberetning.Model
{
    public class MunCellModel
    {
        public UriImageSource ImageSource { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
