using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Newtonsoft.Json;

namespace Flightbook.Generator.Export
{
    internal class FlightbookJsonExporter : IFlightbookJsonExporter
    {
        public string CreateFlightbookJson(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways, List<CountryInfo> worldCountries, Config configuration)
        {
            List<Aircraft> aircrafts = ExtractAircrafts(logEntries);
            List<Airport> airports = ExtractAirports(logEntries, worldAirports, worldRunways);
            List<Country> countries = ExtractCountries(airports, worldCountries);
            List<FlightTimeMonth> flightTimeStatistics = GetFlightTimeStatistics(logEntries);

            Models.Flightbook.Flightbook flightbook = new() {ParentPage = configuration.ParentPage?.Length > 0 ? configuration.ParentPage : null, Aircrafts = aircrafts, Airports = airports, Countries = countries, FlightTimeMonths = flightTimeStatistics};

            return JsonConvert.SerializeObject(flightbook);
        }

        private List<Aircraft> ExtractAircrafts(List<LogEntry> logEntries)
        {
            List<Aircraft> aircrafts = logEntries.Select(l => l.AircraftRegistration).Distinct().Select(a => new Aircraft {Registration = a}).ToList();

            aircrafts.ForEach(aircraft =>
            {
                LogEntry[] filteredLogEntries = logEntries.Where(l => l.AircraftRegistration == aircraft.Registration).ToArray();

                aircraft.FirstFlown = filteredLogEntries.Select(l => l.LogDate).Min();
                aircraft.LastFlown = filteredLogEntries.Select(l => l.LogDate).Max();
                aircraft.Type = filteredLogEntries.Select(l => l.AircraftType).First();
                aircraft.NumberOfFlights = filteredLogEntries.Length;
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

        private List<Airport> ExtractAirports(List<LogEntry> logEntries, List<AirportInfo> worldAirports, List<RunwayInfo> worldRunways)
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
                airport.IsoCountry = airportInfo.IsoCountry;
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
            DateTime startDate = logEntries.Select(l => l.LogDate).Min();
            DateTime endOfMonth = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

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
    }
}
