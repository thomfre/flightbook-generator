namespace Flightbook.Generator.Models.Flightbook
{
    public class FlightStatistics
    {
        public int? Year { get; set; }
        public int AltitudeMax { get; set; }
        public string AltitudeMaxFlight { get; set; }
        public int AltitudeAverage { get; set; }
        public int SpeedMax { get; set; }
        public string SpeedMaxFlight { get; set; }
        public int SpeedAverage { get; set; }
        public double DistanceTotal { get; set; }
        public double DistanceMax { get; set; }
        public string DistanceMaxFlight { get; set; }
        public double DistanceAverage { get; set; }
    }
}
