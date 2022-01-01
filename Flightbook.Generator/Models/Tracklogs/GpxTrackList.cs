using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public DateTime DateTime { get; set; }

        public string Name { get; set; }
        public string Aircraft { get; set; }
        public string[] Airports { get; set; }
        public bool AsPic { get; set; }
        public bool HasYoutube { get; set; }
        public bool HasBlogpost { get; set; }
        public bool HasFacebookPost { get; set; }
        public bool HasGallery { get; set; }
    }
}
