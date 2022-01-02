using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Registrations;
using Flightbook.Generator.Models.Tracklogs;
using Newtonsoft.Json;

namespace Flightbook.Generator.Export
{
    internal class FlightbookJsonExporter : IFlightbookJsonExporter
    {
        public string CreateFlightbookJson(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways, List<CountryInfo> worldCountries, List<RegionInfo> worldRegions, List<RegistrationPrefix> registrationPrefixes, List<GpxTrack> trackLogs,
            Config configuration)
        {
            List<Aircraft> aircrafts = ExtractAircrafts(logEntries, registrationPrefixes);
            List<Airport> airports = ExtractAirports(logEntries, worldAirports, worldRunways, worldRegions);
            List<Country> countries = ExtractCountries(airports, worldCountries);
            List<FlightTimeMonth> flightTimeStatistics = GetFlightTimeStatistics(logEntries);
            List<FlightStatistics> flightStatistics = GetFlightStatistics(trackLogs);

            Models.Flightbook.Flightbook flightbook = new()
            {
                ParentPage = configuration.ParentPage?.Length > 0 ? configuration.ParentPage : null,
                AirportGallerySearch = configuration.AirportGallerySearch?.Length > 0 ? configuration.AirportGallerySearch : null,
                AircraftGallerySearch = configuration.AircraftGallerySearch?.Length > 0 ? configuration.AircraftGallerySearch : null,
                Aircrafts = aircrafts,
                Airports = airports,
                Countries = countries,
                FlightTimeMonths = flightTimeStatistics,
                FlightStatistics = flightStatistics
            };

            return JsonConvert.SerializeObject(flightbook);
        }

        private List<Aircraft> ExtractAircrafts(List<LogEntry> logEntries, List<RegistrationPrefix> registrationPrefixes)
        {
            List<Aircraft> aircrafts = logEntries.Select(l => l.AircraftRegistration).Distinct().Select(a => new Aircraft {Registration = a}).ToList();

            aircrafts.ForEach(aircraft =>
            {
                LogEntry[] filteredLogEntries = logEntries.Where(l => l.AircraftRegistration == aircraft.Registration).ToArray();

                string prefix = aircraft.Registration.Split("-")[0] + "-";

                aircraft.IsoCountry = registrationPrefixes.FirstOrDefault(r => r.Prefix == prefix)?.CountryCode ?? "ZZ";
                aircraft.FirstFlown = filteredLogEntries.Select(l => l.LogDate).Min();
                aircraft.LastFlown = filteredLogEntries.Select(l => l.LogDate).Max();
                aircraft.Type = filteredLogEntries.Select(l => l.AircraftType).First();
                aircraft.DistinctFlightDates = filteredLogEntries.Select(l => l.LogDate).Distinct().Count();
                aircraft.NumberOfFlights = filteredLogEntries.Length;
                aircraft.NumberOfAirports = filteredLogEntries.SelectMany(l => new List<string> {l.From, l.To}.Concat(l.Via ?? new string[0])).Distinct().Count();
                aircraft.AsDual = filteredLogEntries.Any(l => l.DualMinutes > 0);
                aircraft.AsPic = filteredLogEntries.Any(l => l.PicMinutes > 0);
                aircraft.Picture = GetAircraftPicture(aircraft.Registration);
            });

            return aircrafts;
        }

        private string GetAircraftPicture(string registration)
        {
            if (!File.Exists($@"config\aircrafts\{registration.ToLowerInvariant()}.jpg"))
            {
                return null;
            }

            return $"/aircrafts/{registration.ToLowerInvariant()}.jpg";
        }

        private List<Airport> ExtractAirports(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways, List<RegionInfo> worldRegions)
        {
            List<Airport> airports = logEntries.Select(l => l.From)
                .Concat(logEntries.Select(l => l.To))
                .Concat(logEntries.Where(l => l.Via != null).SelectMany(l => l.Via))
                .Distinct()
                .Select(a => new Airport {Icao = a}).ToList();

            airports.ForEach(airport =>
            {
                LogEntry[] filteredLogEntries = logEntries.Where(l => l.From == airport.Icao || l.To == airport.Icao || l.Via != null && l.Via.Contains(airport.Icao)).ToArray();

                airport.FirstVisited = filteredLogEntries.Select(l => l.LogDate).Min();
                airport.LastVisited = filteredLogEntries.Select(l => l.LogDate).Max();
                airport.DistinctVisitDates = filteredLogEntries.Select(l => l.LogDate).Distinct().Count();
                airport.TotalFlights = filteredLogEntries.Length;
                airport.Aircrafts = filteredLogEntries.Select(l => l.AircraftRegistration).Distinct().ToArray();
                airport.AsDual = filteredLogEntries.Any(l => l.DualMinutes > 0);
                airport.AsPic = filteredLogEntries.Any(l => l.PicMinutes > 0);
                airport.AsFrom = filteredLogEntries.Any(l => l.From == airport.Icao);
                airport.AsTo = filteredLogEntries.Any(l => l.To == airport.Icao);
                airport.AsVia = filteredLogEntries.Any(l => l.Via != null && l.Via.Contains(airport.Icao));

                airport.Picture = GetAirportPicture(airport.Icao);

                AirportInfo airportInfo = worldAirports.Find(ai => string.Equals(ai.IcaoCode, airport.Icao, StringComparison.InvariantCultureIgnoreCase));
                if (airportInfo == null)
                {
                    return;
                }

                airport.Name = airportInfo.Name;
                airport.Iata = airportInfo.IataCode;
                airport.Type = GetAirportType(airportInfo.Type);
                airport.Wikipedia = airportInfo.WikipediaLink;
                airport.IsoCountry = airportInfo.IsoCountry;
                airport.Region = worldRegions.Where(r => r.Code == airportInfo.IsoRegion).Select(r => r.Name).FirstOrDefault();
                airport.FieldElevation = airportInfo.FieldElevation;
                if (airportInfo.Latitude.HasValue && airportInfo.Longitude.HasValue)
                {
                    airport.Coordinates = new[] {airportInfo.Latitude.Value, airportInfo.Longitude.Value};
                }
            });

            return airports;
        }

        private string GetAirportPicture(string icao)
        {
            if (!File.Exists($@"config\airports\{icao.ToLowerInvariant()}.jpg"))
            {
                return null;
            }

            return $"/airports/{icao.ToLowerInvariant()}.jpg";
        }

        private string GetAirportType(string airportType)
        {
            switch (airportType)
            {
                case "large_airport":
                    return "Large airport";
                case "medium_airport":
                    return "Medium airport";
                case "small_airport":
                    return "Small airport";
                case "seaplane_base":
                    return "Seaplane base";
                case "heliport":
                    return "Heliport";
                case "closed":
                    return "Closed airport";
                default:
                    return "Unknown";
            }
        }

        private List<Country> ExtractCountries(List<Airport> airports, List<CountryInfo> worldCountries)
        {
            List<Country> countries = airports.Select(c => c.IsoCountry).Distinct().Select(c => new Country {Iso = c}).ToList();

            countries.ForEach(country =>
            {
                CountryInfo countryInfo = worldCountries.Find(ci => string.Equals(ci.Code, country.Iso, StringComparison.InvariantCultureIgnoreCase));
                if (countryInfo == null)
                {
                    return;
                }

                country.Name = countryInfo.Name;
            });

            return countries;
        }

        private List<FlightTimeMonth> GetFlightTimeStatistics(List<LogEntry> logEntries)
        {
            DateTime startDate = logEntries.Min(l => l.LogDate);
            DateTime endOfMonth = logEntries.Max(l => l.LogDate);

            List<FlightTimeMonth> months = new();

            while (startDate <= endOfMonth)
            {
                List<LogEntry> filteredLogEntries = logEntries.Where(l => l.LogDate.Year == startDate.Year && l.LogDate.Month == startDate.Month).ToList();

                months.Add(new FlightTimeMonth
                {
                    Month = startDate.ToString("yyyy-MM"),
                    FlightTimeMinutes = filteredLogEntries.Sum(l => l.TotalMinutes),
                    DualMinutes = filteredLogEntries.Sum(l => l.DualMinutes),
                    PicMinutes = filteredLogEntries.Sum(l => l.PicMinutes),
                    NumberOfFlights = filteredLogEntries.Count
                });

                startDate = startDate.AddMonths(1);
            }

            return months;
        }

        private List<FlightStatistics> GetFlightStatistics(List<GpxTrack> trackLogs)
        {
            int startYear = trackLogs.Select(l => l.DateTime).Min().Year;
            int endYear = trackLogs.Select(l => l.DateTime).Max().Year;

            List<FlightStatistics> statistics = new()
            {
                new FlightStatistics
                {
                    AltitudeMax = trackLogs.Max(t => t.AltitudeMax),
                    AltitudeMaxFlight = trackLogs.OrderByDescending(t => t.AltitudeMax).Select(t => t.Filename).FirstOrDefault(),
                    AltitudeAverage = (int) trackLogs.Average(t => t.AltitudeMax),
                    SpeedMax = trackLogs.Max(t => t.SpeedMax),
                    SpeedMaxFlight = trackLogs.OrderByDescending(t => t.SpeedMax).Select(t => t.Filename).FirstOrDefault(),
                    SpeedAverage = (int) trackLogs.Average(t => t.SpeedMax),
                    DistanceTotal = trackLogs.Sum(t => t.TotalDistance),
                    DistanceMax = trackLogs.Max(t => t.TotalDistance),
                    DistanceMaxFlight = trackLogs.OrderByDescending(t => t.TotalDistance).Select(t => t.Filename).FirstOrDefault(),
                    DistanceAverage = trackLogs.Average(t => t.TotalDistance)
                }
            };


            for (int year = startYear; year <= endYear; year++)
            {
                List<GpxTrack> filteredTracks = trackLogs.Where(l => l.DateTime.Year == year).ToList();

                statistics.Add(new FlightStatistics
                {
                    Year = year,
                    AltitudeMax = filteredTracks.Max(t => t.AltitudeMax),
                    AltitudeMaxFlight = filteredTracks.OrderByDescending(t => t.AltitudeMax).Select(t => t.Filename).FirstOrDefault(),
                    AltitudeAverage = (int) filteredTracks.Average(t => t.AltitudeMax),
                    SpeedMax = filteredTracks.Max(t => t.SpeedMax),
                    SpeedMaxFlight = filteredTracks.OrderByDescending(t => t.SpeedMax).Select(t => t.Filename).FirstOrDefault(),
                    SpeedAverage = (int) filteredTracks.Average(t => t.SpeedMax),
                    DistanceTotal = filteredTracks.Sum(t => t.TotalDistance),
                    DistanceMax = filteredTracks.Max(t => t.TotalDistance),
                    DistanceMaxFlight = filteredTracks.OrderByDescending(t => t.TotalDistance).Select(t => t.Filename).FirstOrDefault(),
                    DistanceAverage = filteredTracks.Average(t => t.TotalDistance)
                });
            }

            return statistics;
        }
    }
}
