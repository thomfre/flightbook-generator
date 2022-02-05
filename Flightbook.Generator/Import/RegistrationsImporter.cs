using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Flightbook.Generator.Models.Registrations;

namespace Flightbook.Generator.Import
{
    public interface IRegistrationsImporter
    {
        List<RegistrationPrefix> GetRegistrationPrefixes();
    }

    internal class RegistrationsImporter : IRegistrationsImporter
    {
        public List<RegistrationPrefix> GetRegistrationPrefixes()
        {
            using StreamReader reader = new(@"Data\registrations.csv");
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<RegistrationPrefix>().ToList();
        }
    }
}
