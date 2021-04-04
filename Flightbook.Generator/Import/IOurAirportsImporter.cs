using System.Collections.Generic;
using Flightbook.Generator.Models.OurAirports;

namespace Flightbook.Generator.Import
{
    internal interface IOurAirportsImporter
    {
        List<AirportInfo> GetAirports();
        List<CountryInfo> GetCountries();
    }
}
