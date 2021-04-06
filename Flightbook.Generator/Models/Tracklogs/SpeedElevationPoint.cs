namespace Flightbook.Generator.Models.Tracklogs
{
    public class SpeedElevationPoint
    {
        public SpeedElevationPoint(decimal elevationInFeet, decimal speedInMs)
        {
            Elevation = elevationInFeet * 3.281m;
            Speed = speedInMs * 1.944m;
        }

        public decimal Elevation { get; set; }
        public decimal Speed { get; set; }
    }
}
