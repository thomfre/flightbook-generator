using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Tracklogs;
using GeoJSON.Net.Geometry;
using GeoTimeZone;
using TimeZoneConverter;

namespace Flightbook.Generator.Import
{
    internal class GpxToGeoJsonImporter : IGpxToGeoJsonImporter
    {
        public List<GpxTrack> SearchAndImport(List<LogEntry> logEntries, TracklogExtra[] tracklogExtras, List<AirportInfo> worldAirports)
        {
            string gpxPath = Path.Join(Directory.GetCurrentDirectory(), "config", "Gpx");
            if (!Directory.Exists(gpxPath))
            {
                return new List<GpxTrack>();
            }

            DirectoryInfo configDirectory = new(gpxPath);
            List<FileInfo> files = configDirectory.GetFiles()
                .Where(f => f.Extension.Equals(".Gpx", StringComparison.InvariantCultureIgnoreCase)).ToList();

            return files.Select(file => Convert(file.FullName, logEntries, tracklogExtras, worldAirports)).ToList();
        }

        private GpxTrack Convert(string gpxPath, List<LogEntry> logEntries, TracklogExtra[] tracklogExtras, List<AirportInfo> worldAirports)
        {
            XmlSerializer xmlSerializer = new(typeof(Gpx));
            using TextReader reader = new StringReader(File.ReadAllText(gpxPath));
            Gpx gpx = (Gpx) xmlSerializer.Deserialize(reader);

            if (gpx == null)
            {
                return null;
            }

            Gpx.TrackPoint[] trackpoints = gpx.Tracks?.FirstOrDefault()?.Segments?.FirstOrDefault()?.Points;

            IEnumerable<Position> positions = trackpoints?.Select(p => new Position(p.Latitude.ToString(CultureInfo.InvariantCulture), p.Longitude.ToString(CultureInfo.InvariantCulture), p.Elevation.ToString(CultureInfo.InvariantCulture)));

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

            LogEntry logEntry = GetRelevantLogEntry(logEntries, trackStartTime ?? DateTime.Now, worldAirports);

            List<SpeedElevationPoint> speedElevationPoints = trackpoints?.Select(p => new SpeedElevationPoint(p.Elevation, p.Speed)).ToList();

            return new GpxTrack
            {
                Date = date,
                DateTime = trackStartTime ?? DateTime.Now,
                Name = gpx.Tracks?.FirstOrDefault()?.Name,
                Aircraft = tracklogExtra?.Aircraft ?? GetAircraft(logEntries, trackStartTime ?? DateTime.Now, logEntry),
                From = logEntry.From,
                To = logEntry.To,
                Via = logEntry.Via,
                AsPic = logEntry.PicMinutes > 0,
                Youtube = tracklogExtra?.Youtube,
                Blogpost = tracklogExtra?.Blogpost,
                FacebookPost = tracklogExtra?.FacebookPost,
                Gallery = tracklogExtra?.Gallery,
                AltitudeMax = speedElevationPoints?.Max(p => (int) p.Elevation) ?? 0,
                AltitudeAverage = speedElevationPoints != null ? (int) speedElevationPoints?.Average(p => p.Elevation) : 0,
                SpeedMax = speedElevationPoints?.Max(p => (int) p.Speed) ?? 0,
                SpeedAverage = speedElevationPoints != null ? (int) speedElevationPoints?.Average(p => p.Speed) : 0,
                TotalDistance = totalDistance,
                GeoJson = lineString,
                SpeedElevationPoints = speedElevationPoints
            };
        }

        private LogEntry GetRelevantLogEntry(IEnumerable<LogEntry> logEntries, DateTime trackStartTime, List<AirportInfo> worldAirports)
        {
            List<LogEntry> relevantLogEntries = logEntries.Where(l => l.LogDate.Date == trackStartTime.Date).ToList();

            if (!relevantLogEntries.Any())
            {
                return null;
            }

            if (relevantLogEntries.Count == 1)
            {
                return relevantLogEntries.First();
            }

            LogEntry mostRelevantLogEntry = relevantLogEntries
                .OrderBy(l => Math.Abs((GetLogTime(l.LogDate, l.Departure, l.From, worldAirports) - trackStartTime).TotalMinutes))
                .FirstOrDefault();

            return mostRelevantLogEntry;
        }

        private static DateTime GetLogTime(DateTime logDate, string departureTime, string fromAirport, IEnumerable<AirportInfo> worldAirports)
        {
            return DateTime.Parse($"{logDate.Date.ToShortDateString()} {departureTime.Split(" ").FirstOrDefault()}{GetTimeZoneOffset(logDate, departureTime, fromAirport, worldAirports)}")
                .ToUniversalTime();
        }

        private static string GetTimeZoneOffset(DateTime logDate, string departureTime, string fromAirport, IEnumerable<AirportInfo> worldAirports)
        {
            string timeType = departureTime.Split(" ").LastOrDefault()?.ToUpperInvariant().Trim();

            if (string.IsNullOrEmpty(timeType) || timeType is "Z" or "ZULU" or "UTC")
            {
                return "Z";
            }

            AirportInfo airport = worldAirports.FirstOrDefault(a => a.IcaoCode == fromAirport);

            if (airport?.Latitude == null || airport.Longitude == null)
            {
                return "Z";
            }

            if (timeType != "LOCAL")
            {
                return "Z";
            }

            string ianaZone = TimeZoneLookup.GetTimeZone((double) airport.Latitude, (double) airport.Longitude).Result;
            TimeZoneInfo timezone = TZConvert.GetTimeZoneInfo(ianaZone);
            DateTimeOffset convertedTime = TimeZoneInfo.ConvertTime(new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 00, 00), timezone);

            return $"{(convertedTime.Offset.TotalMinutes < 0 ? "" : "+")}{convertedTime.Offset}";
        }

        private static string GetAircraft(IEnumerable<LogEntry> logEntries, DateTime trackStartTime, LogEntry mostRelevantLogEntry)
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

            if (relevantLogEntries.Select(l => l.AircraftRegistration).Distinct().Count() == 1)
            {
                return relevantLogEntries.First().AircraftRegistration;
            }

            return mostRelevantLogEntry?.AircraftRegistration ?? "";
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
