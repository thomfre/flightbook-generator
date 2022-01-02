using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Flightbook
    {
        [CanBeNull]
        public string ParentPage { get; set; } = null;

        [CanBeNull]
        public string AirportGallerySearch { get; set; } = null;

        [CanBeNull]
        public string AircraftGallerySearch { get; set; } = null;

        [CanBeNull]
        public string FlickrProxyUrl { get; set; } = null;

        [UsedImplicitly]
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        public List<Aircraft> Aircrafts { get; set; }
        public List<Airport> Airports { get; set; }
        public List<Country> Countries { get; set; }
        public List<FlightTimeMonth> FlightTimeMonths { get; set; }
        public List<FlightStatistics> FlightStatistics { get; set; }
    }
}
