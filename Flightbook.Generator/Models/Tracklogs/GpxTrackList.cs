using System.Collections.Generic;

namespace Flightbook.Generator.Models.Tracklogs
{
    public class GpxTrackList
    {
        public List<GpxTrackInfo> Tracks { get; set; }
    }

    public class GpxTrackInfo
    {
        public string Filename { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string Aircraft { get; set; }
    }
}
