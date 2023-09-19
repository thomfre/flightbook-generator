using System;
using System.Collections.Generic;
using Colorify;
using Flightbook.Generator.Export;
using Flightbook.Generator.Import;
using Flightbook.Generator.Models;
using Flightbook.Generator.Models.Flightbook;
using Flightbook.Generator.Models.OurAirports;
using Flightbook.Generator.Models.Registrations;
using Flightbook.Generator.Models.Tracklogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flightbook.Generator
{
    internal class Application
    {
        private readonly IAirportExporter _airportExporter;
        private readonly IConfigurationLoader _configurationLoader;
        private readonly Format _console;
        private readonly IFlightbookExporter _flightbookExporter;
        private readonly IFlightbookJsonExporter _flightbookJsonExporter;
        private readonly IGpxToGeoJsonImporter _gpxToGeoJsonImporter;
        private readonly ILogbookCsvImporter _logbookCsvImporter;
        private readonly ILogEntryComparisonReport _logEntryComparisonReport;
        private readonly ILogEntryQualityReport _logEntryQualityReport;
        private readonly IOurAirportsImporter _ourAirportsImporter;
        private readonly IRegistrationsImporter _registrationsImporter;
        private readonly ITracklogExporter _tracklogExporter;
        private readonly IHeatmapExporter _heatmapExporter;

        public Application(Format console, IConfigurationLoader configurationLoader, ILogbookCsvImporter logbookCsvImporter, IOurAirportsImporter ourAirportsImporter, IRegistrationsImporter registrationsImporter, IFlightbookJsonExporter flightbookJsonExporter,
            IFlightbookExporter flightbookExporter, IGpxToGeoJsonImporter gpxToGeoJsonImporter, ITracklogExporter tracklogExporter, IHeatmapExporter heatmapExporter, IAirportExporter airportExporter, ILogEntryComparisonReport logEntryComparisonReport, ILogEntryQualityReport logEntryQualityReport)
        {
            _console = console;
            _configurationLoader = configurationLoader;
            _logbookCsvImporter = logbookCsvImporter;
            _ourAirportsImporter = ourAirportsImporter;
            _registrationsImporter = registrationsImporter;
            _flightbookJsonExporter = flightbookJsonExporter;
            _flightbookExporter = flightbookExporter;
            _gpxToGeoJsonImporter = gpxToGeoJsonImporter;
            _tracklogExporter = tracklogExporter;
            _heatmapExporter = heatmapExporter;
            _airportExporter = airportExporter;
            _logEntryComparisonReport = logEntryComparisonReport;
            _logEntryQualityReport = logEntryQualityReport;
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

            DefaultContractResolver contractResolver = new()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {ContractResolver = contractResolver};

            Config configuration = _configurationLoader.GetConfiguration();

            _console.WriteLine("Importing log entries", Colors.txtInfo);
            List<LogEntry> logEntries = _logbookCsvImporter.Import(configuration);
            _console.WriteLine($"Imported {logEntries.Count} log entries", Colors.txtSuccess);

            _console.WriteLine("Reading airport/country data", Colors.txtInfo);
            List<AirportInfo> worldAirports = _ourAirportsImporter.GetAirports();
            List<RunwayInfo> worldRunways = _ourAirportsImporter.GetRunways();
            List<CountryInfo> worldCountries = _ourAirportsImporter.GetCountries();
            List<RegionInfo> worldRegions = _ourAirportsImporter.GetRegions();
            _console.WriteLine($"Got information for {worldAirports.Count} airports and {worldCountries.Count} countries with {worldRegions.Count} regions", Colors.txtSuccess);

            _console.WriteLine("Reading aircraft registration prefixes", Colors.txtInfo);
            List<RegistrationPrefix> registrationPrefixes = _registrationsImporter.GetRegistrationPrefixes();
            _console.WriteLine($"Got information for {registrationPrefixes.Count} prefixes", Colors.txtSuccess);

            _console.WriteLine("Converting GPX files", Colors.txtInfo);
            List<GpxTrack> trackLogs = _gpxToGeoJsonImporter.SearchAndImport(logEntries, configuration.TracklogExtras, worldAirports);
            _console.WriteLine($"Converted {trackLogs.Count} GPX files", Colors.txtSuccess);

            _console.WriteLine("Creating Tracklog data", Colors.txtInfo);
            (string trackLogListJson, Dictionary<string, string> trackLogFileJson) = _tracklogExporter.CreateTracklogFiles(trackLogs);
            _console.WriteLine("Tracklog data crated", Colors.txtSuccess);

            _console.WriteLine("Exporting Heatmap data", Colors.txtInfo);
            string heatmapJson = _heatmapExporter.CreateHeatmapFile(trackLogs);
            _console.WriteLine("heatmap.json exported", Colors.txtSuccess);

            _console.WriteLine("Exporting Flightbook data", Colors.txtInfo);
            string flightbookJson = _flightbookJsonExporter.CreateFlightbookJson(logEntries, worldAirports, worldRunways, worldCountries, worldRegions, registrationPrefixes, configuration.Aircraft, configuration.Operators, trackLogs, configuration);
            _console.WriteLine("flightbook.json exported", Colors.txtSuccess);

            _console.WriteLine("Exporting airports to be collected", Colors.txtInfo);
            string airportsToCollect = _airportExporter.ExportToJson(worldAirports, configuration.CollectingAirportsFromCountries);
            _console.WriteLine("Exported airports", Colors.txtSuccess);

            _console.WriteLine("Updating framework and injecting data", Colors.txtInfo);
            if (_flightbookExporter.Export(flightbookJson, trackLogListJson, trackLogFileJson, heatmapJson, airportsToCollect, configuration.CfAnalytics))
            {
                _console.WriteLine("Framework and data updated, remember to commit and push changes", Colors.txtSuccess);
            }
            else
            {
                _console.WriteLine("Unable to update site with generated data", Colors.txtDanger);
            }

            _console.WriteLine("Generating report of mismatches between logbook and generated track data", Colors.txtInfo);
            int numberOfMismatches = _logEntryComparisonReport.GenerateReport(logEntries, trackLogs, configuration.TracklogExtras);
            if (numberOfMismatches > 0)
            {
                _console.WriteLine($"Mismatches found on {numberOfMismatches} {(numberOfMismatches == 1 ? "entry" : "entries")}", Colors.txtWarning);
            }
            else
            {
                _console.WriteLine("No mismatches found", Colors.txtSuccess);
            }

            _console.WriteLine("Generating log entry quality report", Colors.txtInfo);
            int numberOfLowQuality = _logEntryQualityReport.GenerateReport(logEntries, trackLogs, configuration.IgnoreQualityForEntries);
            if (numberOfLowQuality > 0)
            {
                _console.WriteLine($"Quality issues found on {numberOfLowQuality} {(numberOfLowQuality == 1 ? "entry" : "entries")}", Colors.txtWarning);
            }
            else
            {
                _console.WriteLine("No quality issues found", Colors.txtSuccess);
            }

            _console.WriteLine("");
            _console.WriteLine("Flightbook generation completed, press <Enter> to exit", Colors.txtPrimary);

            _console.ResetColor();

            Console.ReadLine();
        }
    }
}
