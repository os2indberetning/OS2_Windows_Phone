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
