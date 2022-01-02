﻿namespace Flightbook.Generator.Models.Flightbook
{
    public class Config
    {
        public string ParentPage { get; set; }
        public string AirportGallerySearch { get; set; }
        public string AircraftGallerySearch { get; set; }
        public string FlickrProxyUrl { get; set; }
        public string[] CollectingAirportsFromCountries { get; set; }
        public string CfAnalytics { get; set; }
        public AircraftInformation[] Aircrafts { get; set; }
        public TracklogExtra[] TracklogExtras { get; set; }
    }

    public class AircraftInformation
    {
        public string Registration { get; set; }
        public string Class { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int? ManufacturedYear { get; set; }
    }

    public class TracklogExtra
    {
        public string Tracklog { get; set; }
        public string Youtube { get; set; }
        public string Blogpost { get; set; }
        public string FacebookPost { get; set; }
        public string Gallery { get; set; }
        public string Aircraft { get; set; }
    }
}
