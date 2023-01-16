using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Flightbook.Generator.Models.OurAirports;
using RegionInfo = Flightbook.Generator.Models.OurAirports.RegionInfo;

namespace Flightbook.Generator.Import
{
    public interface IOurAirportsImporter
    {
        List<AirportInfo> GetAirports();
        List<RunwayInfo> GetRunways();
        List<CountryInfo> GetCountries();
        List<RegionInfo> GetRegions();
    }

    internal class OurAirportsImporter : IOurAirportsImporter
    {
        public List<AirportInfo> GetAirports()
        {
            using StreamReader reader = new(@"Data\airports.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            List<AirportInfo> airportInfos = csv.GetRecords<AirportInfo>().ToList();
            airportInfos.ForEach(a =>
            {
                a.OurAirportsCode = a.IcaoCode;
                if (!string.IsNullOrWhiteSpace(a.GpsCode) && a.GpsCode != a.IcaoCode)
                {
                    a.IcaoCode = a.GpsCode;
                }
            });

            return airportInfos;
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

        public List<RegionInfo> GetRegions()
        {
            using StreamReader reader = new(@"Data\regions.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<RegionInfo>().ToList();
        }
    }
}
