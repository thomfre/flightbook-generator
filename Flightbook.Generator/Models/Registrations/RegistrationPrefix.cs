using CsvHelper.Configuration.Attributes;

namespace Flightbook.Generator.Models.Registrations
{
    public class RegistrationPrefix
    {
        [Index(0)]
        public string Prefix { get; set; }

        [Index(1)]
        public string CountryCode { get; set; }
    }
}
