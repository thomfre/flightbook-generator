using System.Collections.Generic;
using System.Linq;
using Flightbook.Generator.Models.OurAirports;
using Newtonsoft.Json;

namespace Flightbook.Generator.Export
{
    public interface IAirportExporter
    {
        string ExportToJson(List<AirportInfo> airports, string[] countryCodes);
    }

    internal class AirportExporter : IAirportExporter
    {
        public string ExportToJson(List<AirportInfo> worldAirports, string[] countryCodes)
        {
            string[] countryCodesUpperCase = countryCodes.Select(c => c.ToUpperInvariant()).ToArray();

            string[] ignore = {"heliport", "closed"};

            return JsonConvert.SerializeObject(worldAirports.Where(a => countryCodesUpperCase.Contains(a.IsoCountry.ToUpperInvariant()) && !ignore.Contains(a.Type)).ToList());
        }
    }
}
