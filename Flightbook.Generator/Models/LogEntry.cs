using System;
using JetBrains.Annotations;

namespace Flightbook.Generator.Models
{
    public class LogEntry
    {
        public int EntryNumber { get; set; }
        public DateTime LogDate { get; set; }
        public bool AsPic { get; set; }
        public string From { get; set; }
        public string Departure { get; set; }
        public string To { get; set; }
        public string Arrival { get; set; }
        public string[] Via { get; set; }
        public string AircraftRegistration { get; set; }
        public string AircraftType { get; set; }
        public int TotalMinutes { get; set; }
        public int PicMinutes { get; set; }
        public int DualMinutes { get; set; }
        public int InstrumentMinutes { get; set; }
        public int NightMinutes { get; set; }
        public int DayLandings { get; set; }
        public int NightLandings { get; set; }
        public string[] Metars { get; set; }
        public decimal? TrackDistance { get; set; }
        public int? MaxAltitude { get; set; }
        public int? AverageAltitude { get; set; }
        public int? MaxGroundSpeed { get; set; }
        public int? AverageGroundSpeed { get; set; }
        public string FlightbookUrl { get; set; }
        public string[] Squawks { get; set; }
        public string Comments { get; set; }
        public string[] Approaches { get; set; }
        public bool Aborted { get; set; }
        [CanBeNull]
        public Links Links { get; set; }
    }

    public class Links
    {
        [CanBeNull]
        public string Youtube { get; set; }
        [CanBeNull]
        public string Flickr { get; set; }
        [CanBeNull]
        public string Blog { get; set; }
        [CanBeNull]
        public string Facebook { get; set; }
    }
}
