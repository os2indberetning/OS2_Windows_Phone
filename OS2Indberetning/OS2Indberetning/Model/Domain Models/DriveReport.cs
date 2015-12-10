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
