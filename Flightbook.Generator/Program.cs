using Autofac;
using Colorify;
using Colorify.UI;
using Flightbook.Generator.Export;
using Flightbook.Generator.Import;

namespace Flightbook.Generator
{
    internal class Program
    {
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            ContainerBuilder builder = new();

            builder.Register(c => new Format(Theme.Dark)).As<Format>().SingleInstance();
            builder.RegisterType<ConfigurationLoader>().AsImplementedInterfaces();
            builder.RegisterType<LogbookCsvImporter>().AsImplementedInterfaces();
            builder.RegisterType<OurAirportsImporter>().AsImplementedInterfaces();
            builder.RegisterType<RegistrationsImporter>().AsImplementedInterfaces();
            builder.RegisterType<FlightbookJsonExporter>().AsImplementedInterfaces();
            builder.RegisterType<FlightbookExporter>().AsImplementedInterfaces();
            builder.RegisterType<GpxToGeoJsonImporter>().AsImplementedInterfaces();
            builder.RegisterType<TrackLogExporter>().AsImplementedInterfaces();
            builder.RegisterType<AirportExporter>().AsImplementedInterfaces();
            builder.RegisterType<Application>().AsSelf();

            Container = builder.Build();

            Container.Resolve<Application>().Run();
        }
    }
}
