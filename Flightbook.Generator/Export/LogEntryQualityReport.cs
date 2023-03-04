using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator.Export
{
    public interface ILogEntryQualityReport
    {
        int GenerateReport(List<LogEntry> logEntries, List<GpxTrack> trackLogs, int[] ignoreQualityForEntries);
    }

    internal class LogEntryQualityReport : ILogEntryQualityReport
    {
        public int GenerateReport(List<LogEntry> logEntries, List<GpxTrack> trackLogs, int[] ignoreQualityForEntries)
        {
            Dictionary<LogEntry, Dictionary<string, string>> lowQuality = new();

            logEntries.ForEach(entry =>
            {
                if (ignoreQualityForEntries.Contains(entry.EntryNumber))
                {
                    return;
                }

                Dictionary<string, string> problems = new();

                if (!entry.Aborted && trackLogs.All(t => t.LogEntry != entry.EntryNumber))
                {
                    problems.Add("Track", "No track found");
                }
                else
                {
                    if (string.IsNullOrEmpty(entry.FlightbookUrl))
                    {
                        problems.Add("Track", "Track data missing");
                    }
                }

                if (!entry.Aborted && !entry.Metars.Any())
                {
                    problems.Add("METAR", "No parseable METAR found");
                }

                if (!entry.Aborted && !entry.Squawks.Any())
                {
                    problems.Add("Squawk", "No squawks found");
                }

                if (!entry.Aborted && !entry.Approaches.Any())
                {
                    problems.Add("Approaches", "No approaches found");
                }

                if (string.IsNullOrEmpty(entry.Comments))
                {
                    problems.Add("Comments", "No general comments found");
                }

                if (problems.Count > 0)
                {
                    lowQuality.Add(entry, problems);
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
            reportBuilder.AppendLine("# Log entry quality report");
            reportBuilder.AppendLine();

            if (lowQuality.Count == 0)
            {
                reportBuilder.AppendLine("**No problems detected**");
            }
            else
            {
                reportBuilder.AppendLine($"A total of {lowQuality.Sum(q => q.Value.Count)} issues was detected across {lowQuality.Count} log entries");
                reportBuilder.AppendLine();
                reportBuilder.AppendLine("|Entry|Track|Squawk|Approaches|METAR|Comments|");
                reportBuilder.AppendLine("|--|--|--|--|--|--|");

                foreach ((LogEntry entry, Dictionary<string, string> problems) in lowQuality)
                {
                    reportBuilder.AppendLine(
                        $"|{entry.EntryNumber}|{(problems.ContainsKey("Track") ? problems["Track"] : string.Empty)}|{(problems.ContainsKey("Squawk") ? problems["Squawk"] : string.Empty)}|{(problems.ContainsKey("Approaches") ? problems["Approaches"] : string.Empty)}|{(problems.ContainsKey("METAR") ? problems["METAR"] : string.Empty)}|{(problems.ContainsKey("Comments") ? problems["Comments"] : string.Empty)}|");
                }
            }

            reportBuilder.AppendLine();

            File.WriteAllText(@"LogEntryQualityReport.md", reportBuilder.ToString());

            return lowQuality.Count;
        }
    }
}
