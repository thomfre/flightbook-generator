using System.Collections.Generic;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Export
{
    public interface ITracklogExporter
    {
        (string listJson, Dictionary<string, string> trackFiles) CreateTracklogFiles(List<GpxTrack> tracks);
    }
}
