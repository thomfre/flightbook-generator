using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using Flightbook.Generator.Models;

namespace Flightbook.Generator.Import
{
    internal class LogbookCsvImporter : ILogbookCsvImporter
    {
        private readonly Regex _metarRegex = new("^[A-Z]{4} .*=?$", RegexOptions.Multiline);

        public List<LogEntry> Import()
        {
            string logbookPath = GetLogbookPath();

            using StreamReader reader = new(logbookPath);
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            Dictionary<string, string> headerNames = GetHeaderNames(csv.HeaderRecord);

            List<LogEntry> logEntries = new();

            while (csv.Read())
            {
                string via = csv.GetField<string>(headerNames["Via"]);
                int.TryParse(csv.GetField<string>(headerNames["Day Ldg"]), out int dayLandings);
                int.TryParse(csv.GetField<string>(headerNames["Night Ldg"]), out int nightLandings);

                if (HoursMinutesToMinutes(csv.GetField<string>(headerNames["Total Flight Time"])) == 0)
                {
                    continue;
                }

                logEntries.Add(new LogEntry
                {
                    LogDate = csv.GetField<DateTime>(headerNames["Date"]),
                    AsPic = csv.GetField<string>(headerNames["Holder's Operating Capacity"]) == "PIC",
                    From = csv.GetField<string>(headerNames["Departure"]),
                    Departure = csv.GetField<string>(headerNames["DepartureTime"]),
                    To = csv.GetField<string>(headerNames["Arrival"]),
                    Arrival = csv.GetField<string>(headerNames["ArrivalTime"]),
                    Via = string.IsNullOrWhiteSpace(via) ? null : via.Split(","),
                    AircraftRegistration = csv.GetField<string>(headerNames["Aircraft Registration"]),
                    AircraftType = csv.GetField<string>(headerNames["Aircraft Type"]),
                    TotalMinutes = HoursMinutesToMinutes(csv.GetField<string>(headerNames["Total Flight Time"])),
                    PicMinutes = HoursMinutesToMinutes(csv.GetField<string>(headerNames["PIC"])),
                    DualMinutes = HoursMinutesToMinutes(csv.GetField<string>(headerNames["Dual Received"])),
                    InstrumentMinutes = HoursMinutesToMinutes(csv.GetField<string>(headerNames["Instrument Flying"])),
                    NightMinutes = HoursMinutesToMinutes(csv.GetField<string>(headerNames["Flight time Night"])),
                    DayLandings = dayLandings,
                    NightLandings = nightLandings,
                    Metars = GetMetars(csv.GetField<string>(headerNames["Weather Conditions"]))
                });
            }

            return logEntries;
        }

        private string[] GetMetars(string fieldValue)
        {
            return _metarRegex.Matches(fieldValue).Select(m => m.Value.Trim()).ToArray();
        }

        private string GetLogbookPath()
        {
            string configPath = Path.Join(Directory.GetCurrentDirectory(), "config");
            DirectoryInfo configDirectory = new(configPath);
            IOrderedEnumerable<FileInfo> files = configDirectory.GetFiles()
                .Where(f => f.Extension.Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(f => f.LastWriteTimeUtc);

            return files.First().FullName;
        }

        private int HoursMinutesToMinutes(string hoursMinutes)
        {
            if (string.IsNullOrWhiteSpace(hoursMinutes))
            {
                return 0;
            }

            string[] parts = hoursMinutes.Split(":");
            return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
        }

        private Dictionary<string, string> GetHeaderNames(string[] header)
        {
            Dictionary<string, string> headerNameMappings = new()
            {
                {"Holder's Operating Capacity", header.FirstOrDefault(r => r == "Holder's Operating Capacity")},
                {"Via", header.FirstOrDefault(r => r == "Via")},
                {"Day Ldg", header.FirstOrDefault(r => r == "Day Ldg")},
                {"Night Ldg", header.FirstOrDefault(r => r == "Night Ldg")},
                {"Date", header.FirstOrDefault(r => r == "Date")},
                {"Departure", header.FirstOrDefault(r => r == "Departure")},
                {"DepartureTime", header.FirstOrDefault(r => r is "Time" or "Departure Time" or "DepartureTime")},
                {"Arrival", header.FirstOrDefault(r => r == "Arrival")},
                {"ArrivalTime", header.LastOrDefault(r => r is "Time" or "Arrival Time" or "ArrivalTime")},
                {"Aircraft Registration", header.FirstOrDefault(r => r == "Aircraft Registration")},
                {"Aircraft Type", header.FirstOrDefault(r => r == "Aircraft Type")},
                {"Flight time Night", header.FirstOrDefault(r => r == "Flight time Night")},
                {"Total Flight Time", header.FirstOrDefault(r => r == "Total Flight Time")},
                {"PIC", header.FirstOrDefault(r => r is "PIC" or "Flight time PIC")},
                {"Dual Received", header.FirstOrDefault(r => r is "Dual Received" or "Dual" or "Flight time Dual" or "Flight time Dual Received")},
                {"Instrument Flying", header.FirstOrDefault(r => r is "Instrument Flying" or "Instrument" or "Flight time Instrument Flying")},
                {"Weather Conditions", header.FirstOrDefault(r => r is "Weather Conditions")}
            };


            return headerNameMappings;
        }
    }
}
