using System;
using System.Xml.Serialization;

namespace Flightbook.Generator.Models
{
    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Gpx
    {
        [XmlElement(ElementName = "trk")]
        public Track[] Tracks { get; set; }

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
    }
}
