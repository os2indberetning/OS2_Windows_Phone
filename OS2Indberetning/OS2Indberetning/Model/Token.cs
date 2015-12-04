using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS2Indberetning.Model
{
    public class Token
    {
        public int Id { get; set; }
        //public int ProfileId { get; set; }
        public string GuId { get; set; }
        public string TokenString { get; set; }
        public int Status { get; set; }
    }
}
