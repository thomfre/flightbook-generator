using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Flightbook
    {
        public string? ParentPage { get; set; } = null;

        [UsedImplicitly]
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        public List<Aircraft> Aircrafts { get; set; }
        public List<Airport> Airports { get; set; }
        public List<Country> Countries { get; set; }
        public List<FlightTimeMonth> FlightTimeMonths { get; set; }
    }
}
