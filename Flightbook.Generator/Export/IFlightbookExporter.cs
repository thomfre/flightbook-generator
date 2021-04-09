using System.Collections.Generic;

namespace Flightbook.Generator.Export
{
    internal interface IFlightbookExporter
    {
        void Export(string flightbookJson, string trackLogListJson, Dictionary<string, string> trackLogFileJson, string airportsToCollect, string cfAnalytics);
    }
}
