namespace Flightbook.Generator.Export
{
    internal interface IFlightbookExporter
    {
        void Export(string flightbookJson);
    }
}
