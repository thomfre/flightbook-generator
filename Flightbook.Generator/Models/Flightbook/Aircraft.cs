using System;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Aircraft
    {
        public string Registration { get; set; }
        public string IsoCountry { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int? ManufacturedYear { get; set; }
        public Operator Operator { get; set; }
        public DateTime FirstFlown { get; set; }
        public DateTime LastFlown { get; set; }
        public int DistinctFlightDates { get; set; }
        public int NumberOfFlights { get; set; }
        public int NumberOfAirports { get; set; }
        public bool AsDual { get; set; }
        public bool AsPic { get; set; }
        public bool AsInstructor { get; set; }
        public string Picture { get; set; }
    }
}
