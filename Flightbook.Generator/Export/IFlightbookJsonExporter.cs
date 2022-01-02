using System.Collections.Generic;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Registrations;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Export
{
    internal interface IFlightbookJsonExporter
    {
        string CreateFlightbookJson(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways, List<CountryInfo> countries, List<RegionInfo> worldRegions, List<RegistrationPrefix> registrationPrefixes, AircraftInformation[] aircraftInformations, List<GpxTrack> trackLogs, Config configuration);
    }
}
