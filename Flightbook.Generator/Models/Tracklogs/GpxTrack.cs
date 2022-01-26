using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flightbook.Generator.Models.Tracklogs
{
    public class GpxTrack
    {
        [JsonIgnore]
        public string Filename { get; set; }
        public string Date { get; set; }

        [JsonIgnore]
        public DateTime DateTime { get; set; }

        public string Name { get; set; }
        public string Aircraft { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string[] Via { get; set; }
        public bool AsPic { get; set; }
        public string Youtube { get; set; }
        public string Blogpost { get; set; }
        public string FacebookPost { get; set; }
        public string Gallery { get; set; }
        public int AltitudeMax { get; set; }
        public int AltitudeAverage { get; set; }
        public int SpeedMax { get; set; }
        public int SpeedAverage { get; set; }
        public double TotalDistance { get; set; }
        public string[] Metars { get; set; }
        public object GeoJson { get; set; }
        public List<SpeedElevationPoint> SpeedElevationPoints { get; set; }
    }
}
