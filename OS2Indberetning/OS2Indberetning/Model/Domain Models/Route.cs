using System.Collections.Generic;

namespace OS2Indberetning.Model
{
    public class Route
    {
        public Route()
        {
            GPSCoordinates = new List<GPSCoordinate>();
        }
        public int Id { get; set; }
        public int DriveReportId { get; set; }
        public double TotalDistance { get; set; }
        public List<GPSCoordinate> GPSCoordinates { get; set; } 
    }
}
