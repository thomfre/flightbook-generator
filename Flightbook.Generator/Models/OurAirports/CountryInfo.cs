using CsvHelper.Configuration.Attributes;

namespace Flightbook.Generator.Models.OurAirports
{
    public class CountryInfo
    {
        [Index(0)]
        public int Id { get; set; }

        [Index(1)]
        public string Code { get; set; }

        [Index(2)]
        public string Name { get; set; }

        [Index(3)]
        public string Continent { get; set; }

        [Index(4)]
        public string WikipediaLink { get; set; }

        [Index(5)]
        public string Keywords { get; set; }
    }
}
