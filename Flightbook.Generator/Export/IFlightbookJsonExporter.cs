using System.Collections.Generic;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;

namespace Flightbook.Generator.Export
{
    internal interface IFlightbookJsonExporter
    {
        string CreateFlightbookJson(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways, List<CountryInfo> countries, Config configuration);
    }
}
