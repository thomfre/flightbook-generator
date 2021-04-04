using System;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Airport
    {
        public string Name { get; set; }
        public string Icao { get; set; }
        public string IsoCountry { get; set; }
        public decimal[] Coordinates { get; set; }
        public DateTime FirstVisited { get; set; }
        public DateTime LastVisited { get; set; }
        public int DistinctVisitDates { get; set; }
        public string Picture { get; set; }
    }
}
