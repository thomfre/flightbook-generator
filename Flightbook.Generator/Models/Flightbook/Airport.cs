using System;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Airport
    {
        public string Name { get; set; }
        public string Icao { get; set; }
        public string Iata { get; set; }
        public string IsoCountry { get; set; }
        public string Region { get; set; }
        public decimal[] Coordinates { get; set; }
        public int? FieldElevation { get; set; }
        public string Type { get; set; }
        public string Wikipedia { get; set; }
        public DateTime FirstVisited { get; set; }
        public DateTime LastVisited { get; set; }
        public bool AsDual { get; set; }
        public bool AsPic { get; set; }
        public bool AsFrom { get; set; }
        public bool AsTo { get; set; }
        public bool AsVia { get; set; }
        public int DistinctVisitDates { get; set; }
        public int TotalFlights { get; set; }
        public string[] Aircrafts { get; set; }
        public string Picture { get; set; }
    }
}
