using System;
using System.Xml.Serialization;

namespace Flightbook.Generator.Models
{
    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Gpx
    {
        [XmlElement(ElementName = "trk")]
        public Track[] Tracks { get; set; }

        [XmlElement(ElementName = "rte")]
        public Route[] Routes { get; set; }

        [XmlRoot(ElementName = "rte")]
        public class Route
        {
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "number")]
            public int Number { get; set; }

            [XmlElement(ElementName = "rtept")]
            public RoutePoint[] Points { get; set; }
        }

        [XmlRoot(ElementName = "rtept")]
        public class RoutePoint
        {
            [XmlAttribute(AttributeName = "lat")]
            public decimal Latitude { get; set; }

            [XmlAttribute(AttributeName = "lon")]
            public decimal Longitude { get; set; }

            [XmlElement(ElementName = "ele")]
            public decimal Elevation { get; set; }

            [XmlElement(ElementName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "sym")]
            public string Sym { get; set; }

            [XmlElement(ElementName = "extensions")]
            public SkyDemonRoutePointExtensions SkdExtensions { get; set; }
        }

        [XmlRoot(ElementName = "trk")]
        public class Track
        {
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "trkseg")]
            public TrackSegment[] Segments { get; set; }
        }

        [XmlRoot(ElementName = "trkseg")]
        public class TrackSegment
        {
            [XmlElement(ElementName = "trkpt")]
            public TrackPoint[] Points { get; set; }
        }

        [XmlRoot(ElementName = "trkpt")]
        public class TrackPoint
        {
            [XmlAttribute(AttributeName = "lat")]
            public decimal Latitude { get; set; }

            [XmlAttribute(AttributeName = "lon")]
            public decimal Longitude { get; set; }

            [XmlElement(ElementName = "ele")]
            public decimal Elevation { get; set; }

            [XmlElement(ElementName = "speed")]
            public decimal Speed { get; set; }

            [XmlElement(ElementName = "time")]
            public DateTime Time { get; set; }
        }

        [XmlRoot(ElementName = "extensions")]
        public class SkyDemonRoutePointExtensions
        {
            [XmlElement(ElementName = "level", Namespace = "http://www.skydemon.aero/gpxextensions")]
            public SkyDemonLevel Level { get; set; }
        }

        [XmlRoot(ElementName = "level", Namespace = "http://www.skydemon.aero/gpxextensions")]
        public class SkyDemonLevel
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "value")]
            public int Value { get; set; }
        }
    }
}
