using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Flightbook.Generator.Models;

namespace Flightbook.Generator.Import
{
    internal class LogbookCsvImporter : ILogbookCsvImporter
    {
        public List<LogEntry> Import()
        {
            string logbookPath = GetLogbookPath();

            using StreamReader reader = new(logbookPath);
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            List<LogEntry> logEntries = new();

            while (csv.Read())
            {
                string via = csv.GetField<string>("Via");
                int.TryParse(csv.GetField<string>("Day Ldg"), out int dayLandings);
                int.TryParse(csv.GetField<string>("Night Ldg"), out int nightLandings);

                logEntries.Add(new LogEntry
                {
                    LogDate = csv.GetField<DateTime>("Date"),
                    From = csv.GetField<string>("Departure"),
                    To = csv.GetField<string>("Arrival"),
                    Via = string.IsNullOrWhiteSpace(via) ? null : via.Split(","),
                    AircraftRegistration = csv.GetField<string>("Aircraft Registration"),
                    AircraftType = csv.GetField<string>("Aircraft Type"),
                    TotalMinutes = HoursMinutesToMinutes(csv.GetField<string>("Total Flight Time")),
                    PicMinutes = HoursMinutesToMinutes(csv.GetField<string>("PIC")),
                    DualMinutes = HoursMinutesToMinutes(csv.GetField<string>("Dual Received")),
                    InstrumentMinutes = HoursMinutesToMinutes(csv.GetField<string>("Instrument Flying")),
                    DayLandings = dayLandings,
                    NightLandings = nightLandings
                });
            }

            return logEntries;
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
    }
}
