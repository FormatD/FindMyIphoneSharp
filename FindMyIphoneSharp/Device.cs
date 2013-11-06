
using System;
using FindMyIphoneSharp.Models;

namespace FindMyIphoneSharp
{
    public class Device
    {

        public bool CanWipeAfterLock { get; set; }
        public bool LocFoundEnabled { get; set; }
        public LocationInfo LocationInfo { get; set; }
        public string DeviceModel { get; set; }
        public RemoteLockInfo RemoteLockInfo { get; set; }
        public bool ActivationLocked { get; set; }
        public string RawDeviceModel { get; set; }
        public bool LocationEnabled { get; set; }
        public string ModelDisplayName { get; set; }
        public string Id { get; set; }
        public bool LostModeCapable { get; set; }
        public string DeviceDisplayName { get; set; }
        public bool DarkWake { get; set; }
        public bool LocationCapable { get; set; }
        public long MaxMsgChar { get; set; }
        public string Name { get; set; }
        public double BatteryLevel { get; set; }
        public FeaturesInfo FeaturesInfo { get; set; }
        public string DeviceClass { get; set; }
        public bool WipeInProgress { get; set; }
        public long PasscodeLength { get; set; }
        public bool IsMac { get; set; }
        public bool IsLocating { get; set; }
        public string DeviceColor { get; set; }
        public string BatteryStatus { get; set; }
        public string DeviceStatus { get; set; }
        public DateTime? LockedTimestamp { get; set; }
        public DateTime? WipedTimestamp { get; set; }
        public MsgInfo MsgInfo { get; set; }
        public bool LostModeEnabled { get; set; }
        public string LostTimestamp { get; set; }
        public LostDeviceInfo LostDeviceInfo { get; set; }
        public UserInfo UserInfo { get; set; }

        public override String ToString()
        {
            return "Device{" +
                   "name='" + Name + '\'' +
                   '}';
        }
    }
}
