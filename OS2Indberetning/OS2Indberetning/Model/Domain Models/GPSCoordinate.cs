namespace OS2Indberetning.Model
{
    public class GPSCoordinate
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string IsViaPoint = "false";
    }
}