using Flightbook.Generator.Models.Flightbook;

namespace Flightbook.Generator
{
    public interface IConfigurationLoader
    {
        Config GetConfiguration();
    }
}
