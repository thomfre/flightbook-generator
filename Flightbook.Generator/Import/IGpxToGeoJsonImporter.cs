using System.Collections.Generic;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Import
{
    public interface IGpxToGeoJsonImporter
    {
        List<GpxTrack> SearchAndImport(List<LogEntry> logEntries, TracklogExtra[] tracklogExtras, List<AirportInfo> worldAirports);
    }
}
