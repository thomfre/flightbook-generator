using System.Collections.Generic;
using System.Linq;
using Flightbook.Generator.Models.Tracklogs;
using Newtonsoft.Json;

namespace Flightbook.Generator.Export
{
    internal class TrackLogExporter : ITracklogExporter
    {
        public (string listJson, Dictionary<string, string> trackFiles) CreateTracklogFiles(List<GpxTrack> tracks)
        {
            GpxTrackList trackList = new() {Tracks = new List<GpxTrackInfo>(tracks.Count)};
            Dictionary<string, string> trackFiles = new();

            tracks.ForEach(t =>
            {
                string fileName = $"{t.Date}.json";
                int iterator = 2;
                while (trackFiles.ContainsKey(fileName))
                {
                    fileName = $"{t.Date}-{iterator++}.json";
                }

                trackList.Tracks.Add(new GpxTrackInfo
                {
                    Date = t.Date,
                    DateTime = t.DateTime,
                    Name = t.Name,
                    Aircraft = t.Aircraft,
                    Airports = new[] {t.From, t.To}.Concat(t.Via ?? new string[] { }).ToArray(),
                    Filename = fileName,
                    HasYoutube = !string.IsNullOrEmpty(t.Youtube),
                    HasBlogpost = !string.IsNullOrEmpty(t.Blogpost),
                    HasFacebookPost = !string.IsNullOrEmpty(t.FacebookPost),
                    HasGallery = !string.IsNullOrEmpty(t.Gallery)
                });

                trackFiles.Add(fileName, JsonConvert.SerializeObject(t));
            });

            trackList.Tracks = trackList.Tracks.OrderByDescending(f => f.DateTime).ToList();

            return (listJson: JsonConvert.SerializeObject(trackList), trackFiles);
        }
    }
}
