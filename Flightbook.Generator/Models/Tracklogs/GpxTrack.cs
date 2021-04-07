using System.Collections.Generic;

namespace Flightbook.Generator.Models.Tracklogs
{
    public class GpxTrack
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Aircraft { get; set; }
        public double TotalDistance { get; set; }
        public object GeoJson { get; set; }
        public List<SpeedElevationPoint> SpeedElevationPoints { get; set; }
    }
}
