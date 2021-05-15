using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.Tracklogs;
using GeoJSON.Net.Geometry;

namespace Flightbook.Generator.Import
{
    internal class GpxToGeoJsonImporter : IGpxToGeoJsonImporter
    {
        public List<GpxTrack> SearchAndImport(List<LogEntry> logEntries, TracklogExtra[] tracklogExtras)
        {
            string gpxPath = Path.Join(Directory.GetCurrentDirectory(), "config", "Gpx");
            if (!Directory.Exists(gpxPath))
            {
                return new List<GpxTrack>();
            }

            DirectoryInfo configDirectory = new(gpxPath);
            List<FileInfo> files = configDirectory.GetFiles()
                .Where(f => f.Extension.Equals(".Gpx", StringComparison.InvariantCultureIgnoreCase)).ToList();

            return files.Select(file => Convert(file.FullName, logEntries, tracklogExtras)).ToList();
        }

        private GpxTrack Convert(string gpxPath, List<LogEntry> logEntries, TracklogExtra[] tracklogExtras)
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

            IPosition previousCoordinate = null;
            double totalDistance = 0.0;
            foreach (IPosition lineStringCoordinate in lineString.Coordinates)
            {
                if (previousCoordinate != null)
                {
                    double distance = previousCoordinate.DistanceTo(lineStringCoordinate);
                    if (double.IsNaN(distance))
                    {
                        continue;
                    }

                    totalDistance += distance;
                }

                previousCoordinate = lineStringCoordinate;
            }

            DateTime? trackStartTime = gpx.Tracks?.First().Segments.FirstOrDefault()?.Points?.Select(p => p.Time).Min();
            string date = trackStartTime.HasValue ? trackStartTime.Value.ToString("yyyy-MM-dd") : "unknown";

            TracklogExtra tracklogExtra = tracklogExtras.FirstOrDefault(t => t.Tracklog == Path.GetFileName(gpxPath));

            return new GpxTrack
            {
                Date = date,
                Name = gpx.Tracks?.FirstOrDefault()?.Name,
                Aircraft = GetAircraft(logEntries, trackStartTime.Value),
                Youtube = tracklogExtra?.Youtube,
                Blogpost = tracklogExtra?.Blogpost,
                TotalDistance = totalDistance,
                GeoJson = lineString,
                SpeedElevationPoints = gpx.Tracks?.FirstOrDefault()?.Segments?.FirstOrDefault()?.Points.Select(p => new SpeedElevationPoint(p.Elevation, p.Speed)).ToList()
            };
        }

        private string GetAircraft(List<LogEntry> logEntries, DateTime trackStartTime)
        {
            List<LogEntry> relevantLogEntries = logEntries.Where(l => l.LogDate.Date == trackStartTime.Date).ToList();

            if (!relevantLogEntries.Any())
            {
                return null;
            }

            if (relevantLogEntries.Count() == 1)
            {
                return relevantLogEntries.First().AircraftRegistration;
            }

            return relevantLogEntries.Select(l => l.AircraftRegistration).Distinct().Count() == 1 ? relevantLogEntries.First().AircraftRegistration : null;
        }
    }

    public static class PositionExtensions
    {
        public static double DistanceTo(this IPosition baseCoordinates, IPosition targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.NauticalMiles);
        }

        public static double DistanceTo(this IPosition baseCoordinates, IPosition targetCoordinates, UnitOfLength unitOfLength)
        {
            double baseRad = Math.PI * baseCoordinates.Latitude / 180;
            double targetRad = Math.PI * targetCoordinates.Latitude / 180;
            double theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            double thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }

        public class UnitOfLength
        {
            public static UnitOfLength Kilometers = new(1.609344);
            public static UnitOfLength NauticalMiles = new(0.8684);
            public static UnitOfLength Miles = new(1);

            private readonly double _fromMilesFactor;

            private UnitOfLength(double fromMilesFactor)
            {
                _fromMilesFactor = fromMilesFactor;
            }

            public double ConvertFromMiles(double input)
            {
                return input * _fromMilesFactor;
            }
        }
    }
}
