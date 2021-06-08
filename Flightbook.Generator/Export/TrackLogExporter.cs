using System.Collections.Generic;
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

                trackList.Tracks.Add(new GpxTrackInfo {Date = t.Date, Name = t.Name, Aircraft = t.Aircraft, Filename = fileName, HasYoutube = !string.IsNullOrEmpty(t.Youtube), HasBlogpost = !string.IsNullOrEmpty(t.Blogpost)});
                trackFiles.Add(fileName, JsonConvert.SerializeObject(t));
            });

            return (listJson: JsonConvert.SerializeObject(trackList), trackFiles);
        }
    }
}
