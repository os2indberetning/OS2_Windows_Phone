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
