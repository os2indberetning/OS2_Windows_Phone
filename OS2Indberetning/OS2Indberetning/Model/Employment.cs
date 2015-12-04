using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS2Indberetning.Model
{
    public class Employment
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string EmploymentPosition { get; set; }
        public string ManNr { get; set; }
    }
}
