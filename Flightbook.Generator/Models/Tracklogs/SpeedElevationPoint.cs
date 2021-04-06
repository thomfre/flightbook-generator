namespace Flightbook.Generator.Models.Tracklogs
{
    public class SpeedElevationPoint
    {
        public SpeedElevationPoint(decimal elevation, decimal speed)
        {
            Elevation = elevation;
            Speed = speed;
        }

        public decimal Elevation { get; set; }
        public decimal Speed { get; set; }
    }
}
