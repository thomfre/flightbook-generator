using CsvHelper.Configuration.Attributes;

namespace Flightbook.Generator.Models.OurAirports
{
    public class RunwayInfo
    {
        [Index(0)]
        public int Id { get; set; }

        [Index(1)]
        public string AirportRef { get; set; }

        [Index(2)]
        public string AirportIdent { get; set; }

        [Index(3)]
        public string length_ft { get; set; }

        [Index(3)]
        public string width_ft { get; set; }

        [Index(5)]
        public string surface { get; set; }

        [Index(6)]
        public string lighted { get; set; }

        [Index(7)]
        public string closed { get; set; }

        [Index(8)]
        public string le_ident { get; set; }

        [Index(9)]
        public string le_latitude_deg { get; set; }

        [Index(10)]
        public string le_longitude_deg { get; set; }

        [Index(11)]
        public string le_elevation_ft { get; set; }

        [Index(12)]
        public string le_heading_degT { get; set; }

        [Index(13)]
        public string le_displaced_threshold_ft { get; set; }

        [Index(14)]
        public string he_ident { get; set; }

        [Index(15)]
        public string he_latitude_deg { get; set; }

        [Index(16)]
        public string he_longitude_deg { get; set; }

        [Index(17)]
        public string he_elevation_ft { get; set; }

        [Index(18)]
        public string he_heading_degT { get; set; }

        [Index(19)]
        public string he_displaced_threshold_ft { get; set; }
    }
}
