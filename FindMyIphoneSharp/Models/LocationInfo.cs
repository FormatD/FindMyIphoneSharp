using System;
namespace FindMyIphoneSharp.Models
{
    public class LocationInfo
    {
        public DateTime TimeStamp { get; set; }
        public object LocationType { get; set; }
        public bool IsLocationFinished { get; set; }
        public double HorizontalAccuracy { get; set; }
        public string PositionType { get; set; }
        public double Longitude { get; set; }
        public bool IsInaccurate { get; set; }
        public double Latitude { get; set; }
        public bool IsOld { get; set; }
    }
}
