using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Flightbook.Generator.Models.OurAirports;

namespace Flightbook.Generator.Import
{
    internal class OurAirportsImporter : IOurAirportsImporter
    {
        public List<AirportInfo> GetAirports()
        {
            using StreamReader reader = new(@"Data\airports.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<AirportInfo>().ToList();
        }

        public List<RunwayInfo> GetRunways()
        {
            using StreamReader reader = new(@"Data\runways.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<RunwayInfo>().ToList();
        }

        public List<CountryInfo> GetCountries()
        {
            using StreamReader reader = new(@"Data\countries.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<CountryInfo>().ToList();
        }
    }
}
