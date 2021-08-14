using System;
using System.Linq;

namespace Flightbook.Generator.Models
{
    public class LogEntry
    {
        public DateTime LogDate { get; set; }
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
        public int DayLandings { get; set; }
        public int NightLandings { get; set; }
    }
}
