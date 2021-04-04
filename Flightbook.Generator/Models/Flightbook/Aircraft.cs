using System;

namespace Flightbook.Generator.Models.Flightbook
{
    public class Aircraft
    {
        public string Registration { get; set; }
        public string Type { get; set; }
        public DateTime FirstFlown { get; set; }
        public DateTime LastFlown { get; set; }
        public string Picture { get; set; }
    }
}
