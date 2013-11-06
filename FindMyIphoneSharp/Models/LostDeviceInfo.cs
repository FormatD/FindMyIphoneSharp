
namespace FindMyIphoneSharp.Models
{
    public class LostDeviceInfo
    {
        public string StatusCode { get; set; }
        public long CreateTimestamp { get; set; }
        public string Text { get; set; }
        public bool UserText { get; set; }
        public bool StopLostMode { get; set; }
        public string OwnerNbr { get; set; }
        public bool Sound { get; set; }
        public bool EmailUpdates { get; set; }
    }
}
