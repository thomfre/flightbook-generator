using System.Collections.Generic;
using Colorify;
using Flightbook.Generator.Export;
using Flightbook.Generator.Import;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Tracklogs;

namespace Flightbook.Generator
{
    internal class Application
    {
        private readonly Format _console;
        private readonly IFlightbookExporter _flightbookExporter;
        private readonly IFlightbookJsonExporter _flightbookJsonExporter;
        private readonly IGpxToGeoJsonImporter _gpxToGeoJsonImporter;
        private readonly ILogbookCsvImporter _logbookCsvImporter;
        private readonly IOurAirportsImporter _ourAirportsImporter;
        private readonly ITracklogExporter _tracklogExporter;

        public Application(Format console, ILogbookCsvImporter logbookCsvImporter, IOurAirportsImporter ourAirportsImporter, IFlightbookJsonExporter flightbookJsonExporter, IFlightbookExporter flightbookExporter, IGpxToGeoJsonImporter gpxToGeoJsonImporter,
            ITracklogExporter tracklogExporter)
        {
            _console = console;
            _logbookCsvImporter = logbookCsvImporter;
            _ourAirportsImporter = ourAirportsImporter;
            _flightbookJsonExporter = flightbookJsonExporter;
            _flightbookExporter = flightbookExporter;
            _gpxToGeoJsonImporter = gpxToGeoJsonImporter;
            _tracklogExporter = tracklogExporter;
        }

        public void Run()
        {
            _console.WriteLine(@"
  ______ _ _       _     _   _                 _      _____                           _             
  |  ___| (_)     | |   | | | |               | |    |  __ \                         | |            
  | |_  | |_  __ _| |__ | |_| |__   ___   ___ | | __ | |  \/ ___ _ __   ___ _ __ __ _| |_ ___  _ __ 
  |  _| | | |/ _` | '_ \| __| '_ \ / _ \ / _ \| |/ / | | __ / _ | '_ \ / _ | '__/ _` | __/ _ \| '__|
  | |   | | | (_| | | | | |_| |_) | (_) | (_) |   <  | |_\ |  __| | | |  __| | | (_| | || (_) | |   
  \_|   |_|_|\__, |_| |_|\__|_.__/ \___/ \___/|_|\_\  \____/\___|_| |_|\___|_|  \__,_|\__\___/|_|   
              __/ |                                                                                 
             |___/                                                                                  
", Colors.txtInfo);

            _console.DivisionLine('*', Colors.txtMuted);

            _console.WriteLine("Importing log entries", Colors.txtInfo);
            List<LogEntry> logEntries = _logbookCsvImporter.Import();
            _console.WriteLine($"Imported {logEntries.Count} log entries", Colors.txtSuccess);

            _console.WriteLine("Reading airport/country data", Colors.txtInfo);
            List<AirportInfo> worldAirports = _ourAirportsImporter.GetAirports();
            List<CountryInfo> worldCountries = _ourAirportsImporter.GetCountries();
            _console.WriteLine($"Got information for {worldAirports.Count} airports and {worldCountries.Count} countries", Colors.txtSuccess);

            _console.WriteLine("Exporting Flightbook data", Colors.txtInfo);
            string flightbookJson = _flightbookJsonExporter.CreateFlightbookJson(logEntries, worldAirports, worldCountries);
            _console.WriteLine("flightbook.json exported", Colors.txtSuccess);

            _console.WriteLine("Converting GPX files", Colors.txtInfo);
            List<GpxTrack> trackLogs = _gpxToGeoJsonImporter.SearchAndImport();
            _console.WriteLine($"Converted {trackLogs.Count} GPX files", Colors.txtSuccess);

            _console.WriteLine("Exporting Tracklog data", Colors.txtInfo);
            (string trackLogListJson, Dictionary<string, string> trackLogFileJson) = _tracklogExporter.CreateTracklogFiles(trackLogs);
            _console.WriteLine("flightbook.json exported", Colors.txtSuccess);

            _console.WriteLine("Updating framework and injecting data", Colors.txtInfo);
            _flightbookExporter.Export(flightbookJson, trackLogListJson, trackLogFileJson);
            _console.WriteLine("Framework and data updated, remember to commit and push changes", Colors.txtSuccess);

            _console.ResetColor();
        }
    }
}
