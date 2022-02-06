namespace Flightbook.Generator.Models.Flightbook
{
    public class FlightTimeMonth
    {
        public string Month { get; set; }
        public int FlightTimeMinutes { get; set; }
        public int NightMinutes { get; set; }
        public int DualMinutes { get; set; }
        public int PicMinutes { get; set; }
        public int NumberOfFlights { get; set; }
        public int Landings { get; set; }
        public int DualLandings { get; set; }
        public int PicLandings { get; set; }
        public int NightLandings { get; set; }
        public string[] Airports { get; set; }
    }
}
