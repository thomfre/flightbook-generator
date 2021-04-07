using System.IO;
using Flightbook.Generator.Models.Flightbook;
using Newtonsoft.Json;

namespace Flightbook.Generator
{
    internal class ConfigurationLoader : IConfigurationLoader
    {
        public Config GetConfiguration()
        {
            string configPath = Path.Join(Directory.GetCurrentDirectory(), @"config\config.json");

            return !File.Exists(configPath)
                ? new Config {CollectingAirportsFromCountries = new string[0]}
                : JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }
    }
}
