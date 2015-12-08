using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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
