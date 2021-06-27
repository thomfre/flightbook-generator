using System.Collections.Generic;
using Flightbook.Generator.Models.OurAirports;

namespace Flightbook.Generator.Import
{
    internal interface IOurAirportsImporter
    {
        List<AirportInfo> GetAirports();
        List<RunwayInfo> GetRunways();
        List<CountryInfo> GetCountries();
    }
}
