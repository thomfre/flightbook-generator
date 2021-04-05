using System;
using System.Collections.Generic;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Flightbook
    {
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public List<Aircraft> Aircrafts { get; set; }
        public List<Airport> Airports { get; set; }
        public List<Country> Countries { get; set; }
        public List<FlightTimeMonth> FlightTimeMonths { get; set; }
    }
}
