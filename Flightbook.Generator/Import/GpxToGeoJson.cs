using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Tracklogs;
using GeoJSON.Net.Geometry;

namespace Flightbook.Generator.Import
{
    internal class GpxToGeoJsonImporter : IGpxToGeoJsonImporter
    {
        public List<GpxTrack> SearchAndImport()
        {
            string gpxPath = Path.Join(Directory.GetCurrentDirectory(), "config", "Gpx");
            if (!Directory.Exists(gpxPath))
            {
                return new List<GpxTrack>();
            }

            DirectoryInfo configDirectory = new(gpxPath);
            List<FileInfo> files = configDirectory.GetFiles()
                .Where(f => f.Extension.Equals(".Gpx", StringComparison.InvariantCultureIgnoreCase)).ToList();

            return files.Select(file => Convert(file.FullName)).ToList();
        }

        private GpxTrack Convert(string gpxPath)
        {
            XmlSerializer xmlSerializer = new(typeof(Gpx));
            using TextReader reader = new StringReader(File.ReadAllText(gpxPath));
            Gpx gpx = (Gpx) xmlSerializer.Deserialize(reader);

            if (gpx == null)
            {
                return null;
            }

            IEnumerable<Position> positions = gpx.Tracks?.FirstOrDefault()?.Segments?.FirstOrDefault()?.Points
                .Select(p => new Position(p.Latitude.ToString(CultureInfo.InvariantCulture), p.Longitude.ToString(CultureInfo.InvariantCulture), p.Elevation.ToString(CultureInfo.InvariantCulture)));

            LineString lineString = new(positions);

            string date = gpx.Tracks?.First().Segments.FirstOrDefault()?.Points?.Select(p => p.Time).Min().ToString("yyyy-MM-dd");

            return new GpxTrack
            {
                GeoJson = lineString,
                Name = gpx.Tracks?.FirstOrDefault()?.Name,
                Date = date,
                SpeedElevationPoints = gpx.Tracks?.FirstOrDefault()?.Segments?.FirstOrDefault()?.Points.Select(p => new SpeedElevationPoint(p.Elevation, p.Speed)).ToList()
            };
        }
    }
}
