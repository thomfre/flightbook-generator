using System.Collections.Generic;
using Flightbook.Generator.Models.Registrations;

namespace Flightbook.Generator.Import
{
    internal interface IRegistrationsImporter
    {
        List<RegistrationPrefix> GetRegistrationPrefixes();
    }
}
