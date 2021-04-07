using System.Collections.Generic;
using Flightbook.Generator.Models.OurAirports;

namespace Flightbook.Generator.Export
{
    public interface IAirportExporter
    {
        string ExportToJson(List<AirportInfo> airports, string[] countryCodes);
    }
}
