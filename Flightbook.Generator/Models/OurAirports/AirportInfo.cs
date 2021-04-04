using CsvHelper.Configuration.Attributes;

namespace Flightbook.Generator.Models.OurAirports
{
    public class AirportInfo
    {
        [Index(0)]
        public int Id { get; set; }

        [Index(1)]
        public string IcaoCode { get; set; }

        [Index(2)]
        public string Type { get; set; }

        [Index(3)]
        public string Name { get; set; }

        [Index(4)]
        public decimal? Latitude { get; set; }

        [Index(5)]
        public decimal? Longitude { get; set; }

        [Index(6)]
        public int? FieldElevation { get; set; }

        [Index(7)]
        public string Continent { get; set; }

        [Index(8)]
        public string IsoCountry { get; set; }

        [Index(9)]
        public string IsoRegion { get; set; }

        [Index(10)]
        public string Municipality { get; set; }

        [Index(11)]
        public string ScheduledService { get; set; }

        [Index(12)]
        public string GpsCode { get; set; }

        [Index(13)]
        public string IataCode { get; set; }

        [Index(14)]
        public string LocalCode { get; set; }

        [Index(15)]
        public string HomeLink { get; set; }

        [Index(16)]
        public string WikipediaLink { get; set; }

        [Index(17)]
        public string Keywords { get; set; }
    }
}
