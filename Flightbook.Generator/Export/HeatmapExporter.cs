using System;
using System.Collections.Generic;
using System.Linq;
using Flightbook.Generator.Models.Tracklogs;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;

namespace Flightbook.Generator.Export
{
    public interface IHeatmapExporter
    {
        string CreateHeatmapFile(List<GpxTrack> tracks);
    }

    internal class HeatmapExporter : IHeatmapExporter
    {
        private const int CoordinateRounding = 5;

        public string CreateHeatmapFile(List<GpxTrack> tracks)
        {
            var groupedPoints = tracks.GroupBy(t => t.DateTime.Year).Select(g => new
            {
                Year = g.Key, Points = g.Select(t => t.GeoJson as LineString).SelectMany(x => x.Coordinates).GroupBy(x => (Math.Round(x.Latitude, CoordinateRounding), Math.Round(x.Longitude, CoordinateRounding)))
                    .Select(g => new[] {g.Key.Item1, g.Key.Item2, g.Count()})
            });


            return JsonConvert.SerializeObject(groupedPoints);
        }
    }
}
