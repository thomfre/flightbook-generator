using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Export
{
    public interface ILogEntryComparisonReport
    {
        int GenerateReport(List<LogEntry> logEntries, List<GpxTrack> trackLogs, TracklogExtra[] tracklogExtras);
    }

    internal class LogEntryComparisonReport : ILogEntryComparisonReport
    {
        public int GenerateReport(List<LogEntry> logEntries, List<GpxTrack> trackLogs, TracklogExtra[] tracklogExtras)
        {
            Dictionary<LogEntry, string> problems = new();

            trackLogs.ForEach(track =>
            {
                LogEntry logEntry = logEntries.First(l => l.EntryNumber == track.LogEntry);
                List<string> mismatches = new();

                string filename = logEntry.FlightbookUrl.Length > 0 ? logEntry.FlightbookUrl.Split("/").Last() : "";

                if (logEntry.MaxAltitude != track.AltitudeMax)
                {
                    mismatches.Add($"|Max altitude|{FormatValueDisplay(logEntry.MaxAltitude)}|{track.AltitudeMax}|");
                }

                if (logEntry.AverageAltitude != track.AltitudeAverage)
                {
                    mismatches.Add($"|Average altitude|{FormatValueDisplay(logEntry.AverageAltitude)}|{track.AltitudeAverage}|");
                }

                if (logEntry.MaxGroundSpeed != track.SpeedMax)
                {
                    mismatches.Add($"|Max speed|{FormatValueDisplay(logEntry.MaxGroundSpeed)}|{track.SpeedMax}|");
                }

                if (logEntry.AverageGroundSpeed != track.SpeedAverage)
                {
                    mismatches.Add($"|Average speed|{FormatValueDisplay(logEntry.AverageGroundSpeed)}|{track.SpeedAverage}|");
                }

                if (Math.Round(logEntry.TrackDistance ?? 0, 1) != (decimal) Math.Round(track.TotalDistance, 1))
                {
                    mismatches.Add($"|Track distance|{FormatValueDisplay(logEntry.TrackDistance)}|{Math.Round(track.TotalDistance, 1)}|");
                }

                if (filename != track.Filename)
                {
                    mismatches.Add($"|Flightbook URL|{FormatValueDisplay(filename)}|{track.Filename}|");
                }

                List<TracklogExtra> tracklogExtraCandidates = tracklogExtras.Where(t => t.Tracklog.StartsWith(track.Filename)).ToList();
                string[] nameParts = track.Filename.Split("-");
                int fileNumber = nameParts.Length == 3 ? 1 : int.Parse(nameParts[3]);

                TracklogExtra tracklogExtra = tracklogExtraCandidates.Count switch
                {
                    0 => null,
                    1 => tracklogExtraCandidates[0],
                    var _ => tracklogExtraCandidates[fileNumber - 1]
                };

                if (tracklogExtra != null)
                {
                    if (!string.IsNullOrWhiteSpace(tracklogExtra.Youtube) && logEntry.Links?.Youtube?.Length > 0 && tracklogExtra.Youtube != logEntry.Links?.Youtube.FirstOrDefault())
                    {
                        mismatches.Add($"|Youtube URL|{FormatValueDisplay(logEntry.Links?.Youtube)}|{tracklogExtra.Youtube}|");
                    }

                    if (!string.IsNullOrWhiteSpace(tracklogExtra.Blogpost) && !string.IsNullOrWhiteSpace(logEntry.Links?.Blog) && tracklogExtra.Blogpost != logEntry.Links?.Blog)
                    {
                        mismatches.Add($"|Blog URL|{FormatValueDisplay(logEntry.Links?.Blog)}|{tracklogExtra.Blogpost}|");
                    }

                    if (!string.IsNullOrWhiteSpace(tracklogExtra.Gallery) && !string.IsNullOrWhiteSpace(logEntry.Links?.Flickr) && tracklogExtra.Gallery != logEntry.Links?.Flickr)
                    {
                        mismatches.Add($"|Flickr URL|{FormatValueDisplay(logEntry.Links?.Flickr)}|{tracklogExtra.Gallery}|");
                    }

                    if (!string.IsNullOrWhiteSpace(tracklogExtra.FacebookPost) && !string.IsNullOrWhiteSpace(logEntry.Links?.Facebook) && tracklogExtra.FacebookPost != logEntry.Links?.Facebook)
                    {
                        mismatches.Add($"|Facebook URL|{FormatValueDisplay(logEntry.Links?.Facebook)}|{tracklogExtra.FacebookPost}|");
                    }
                }

                if (mismatches.Count > 0)
                {
                    problems.Add(logEntry, string.Join(Environment.NewLine, mismatches));
                }
            });


            StringBuilder reportBuilder = new();
            reportBuilder.AppendLine(@"```
  ______ _ _       _     _   _                 _      _____                           _             
  |  ___| (_)     | |   | | | |               | |    |  __ \                         | |            
  | |_  | |_  __ _| |__ | |_| |__   ___   ___ | | __ | |  \/ ___ _ __   ___ _ __ __ _| |_ ___  _ __ 
  |  _| | | |/ _` | '_ \| __| '_ \ / _ \ / _ \| |/ / | | __ / _ | '_ \ / _ | '__/ _` | __/ _ \| '__|
  | |   | | | (_| | | | | |_| |_) | (_) | (_) |   <  | |_\ |  __| | | |  __| | | (_| | || (_) | |   
  \_|   |_|_|\__, |_| |_|\__|_.__/ \___/ \___/|_|\_\  \____/\___|_| |_|\___|_|  \__,_|\__\___/|_|   
              __/ |                                                                                 
             |___/                                                                                  
```");
            reportBuilder.AppendLine();
            reportBuilder.AppendLine("# Log entry mismatch report");
            reportBuilder.AppendLine();
            foreach (KeyValuePair<LogEntry, string> problem in problems)
            {
                reportBuilder.AppendLine();
                reportBuilder.AppendLine();
                reportBuilder.AppendLine($"## Entry {problem.Key.EntryNumber}");
                reportBuilder.AppendLine();
                reportBuilder.AppendLine($"Problems detected in entry {problem.Key.From}->{problem.Key.To} in {problem.Key.AircraftRegistration} ({problem.Key.LogDate})");
                reportBuilder.AppendLine();
                reportBuilder.AppendLine("|Field|In logbook|In Flightbook|");
                reportBuilder.AppendLine("|-----|----------|-------------|");
                reportBuilder.AppendLine(problem.Value);
            }

            if (problems.Count == 0)
            {
                reportBuilder.AppendLine("**No problems detected**");
            }

            reportBuilder.AppendLine();

            File.WriteAllText(@"LogEntryComparisonReport.md", reportBuilder.ToString());

            return problems.Count;
        }

        private string FormatValueDisplay(object value)
        {
            string stringValue = value?.ToString();
            return string.IsNullOrEmpty(stringValue) ? "_&lt;empty&gt;_" : stringValue;
        }
    }
}
