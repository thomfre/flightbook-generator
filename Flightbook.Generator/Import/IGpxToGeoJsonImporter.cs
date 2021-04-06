using System.Collections.Generic;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Import
{
    public interface IGpxToGeoJsonImporter
    {
        List<GpxTrack> SearchAndImport();
    }
}
