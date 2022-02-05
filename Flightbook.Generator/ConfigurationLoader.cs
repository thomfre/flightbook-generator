using System;
using System.IO;
using Flightbook.Generator.Models.Flightbook;
using Newtonsoft.Json;

namespace Flightbook.Generator
{
    public interface IConfigurationLoader
    {
        Config GetConfiguration();
    }

    internal class ConfigurationLoader : IConfigurationLoader
    {
        public Config GetConfiguration()
        {
            string configPath = Path.Join(Directory.GetCurrentDirectory(), @"config\config.json");

            return !File.Exists(configPath)
                ? new Config {CollectingAirportsFromCountries = Array.Empty<string>()}
                : JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }
    }
}
