using System.Collections.Generic;
using Flightbook.Generator.Models;

namespace Flightbook.Generator.Import
{
    public interface ILogbookCsvImporter
    {
        List<LogEntry> Import();
    }
}
