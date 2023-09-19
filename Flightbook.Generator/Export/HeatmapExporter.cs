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
            var groupedPoints = tracks.Select(t => t.GeoJson as LineString).SelectMany(x => x.Coordinates).GroupBy(x => (Math.Round(x.Latitude, CoordinateRounding), Math.Round(x.Longitude, CoordinateRounding)))
                .Select(g => new {Latitude = g.Key.Item1, Longitude = g.Key.Item2, Count = g.Count()});

            return JsonConvert.SerializeObject(groupedPoints);
        }
    }
}
