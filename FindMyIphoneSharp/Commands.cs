using System;

namespace FindMyIphoneSharp
{
    public class Commands
    {
        private readonly static String INIT_CLIENT = "{\"clientContext\": {\"deviceUDID\": \"0000000000000000000000000000000000000000\", \"inactiveTime\": 2147483647, \"productType\": \"iPad1,1\", \"appName\": \"FindMyiPhone\", \"buildVersion\": \"145\", \"personID\": 0, \"osVersion\": \"4.2.1\", \"appVersion\": \"1.4\"}}";
        private readonly static String SEND_MESSAGE_TMPL = "{\"sound\": %s, \"text\": \"%s\", \"serverContext\": {\"maxLocatingTime\": 90000, \"deviceLoadStatus\": \"203\", \"prefsUpdateTime\": 1276872996660, \"lastSessionExtensionTime\": null, \"clientId\": \"0000000000000000000000000000000000000000\", \"timezone\": {\"previousOffset\": -28800000, \"currentOffset\": -25200000, \"previousTransition\": 1268560799999, \"tzCurrentName\": \"Pacific Daylight Time\", \"tzName\": \"America/Los_Angeles\"}, \"validRegion\": true, \"sessionLifespan\": 900000, \"preferredLanguage\": \"en\", \"hasDevices\": true, \"maxDeviceLoadTime\": 60000, \"callbackIntervalInMS\": 3000}, \"device\": \"%s\", \"clientContext\": {\"deviceUDID\": \"0000000000000000000000000000000000000000\", \"selectedDevice\": \"%s\", \"inactiveTime\": 5911, \"productType\": \"iPad1,1\", \"appName\": \"FindMyiPhone\", \"buildVersion\": \"145\", \"osVersion\": \"3.2\", \"appVersion\": \"1.4\", \"shouldLocate\": false}, \"subject\": \"%s\"}";
        private readonly static String LOCK_DEVICE_TMPL = "{\"device\": \"%s\", \"passcode\": \"%s\", \"serverContext\": {\"maxLocatingTime\": 90000, \"deviceLoadStatus\": \"203\", \"prefsUpdateTime\": 1276872996660, \"lastSessionExtensionTime\": null, \"clientId\": \"0000000000000000000000000000000000000000\", \"timezone\": {\"previousOffset\": -28800000, \"currentOffset\": -25200000, \"previousTransition\": 1268560799999, \"tzCurrentName\": \"Pacific Daylight Time\", \"tzName\": \"America/Los_Angeles\"}, \"validRegion\": true, \"sessionLifespan\": 900000, \"preferredLanguage\": \"en\", \"hasDevices\": true, \"maxDeviceLoadTime\": 60000, \"callbackIntervalInMS\": 3000}, \"oldPasscode\": \"\", \"clientContext\": {\"deviceUDID\": \"0000000000000000000000000000000000000000\", \"selectedDevice\": \"%s\", \"inactiveTime\": 5911, \"productType\": \"iPad1,1\", \"appName\": \"FindMyiPhone\", \"buildVersion\": \"145\", \"osVersion\": \"3.2\", \"appVersion\": \"1.4\", \"shouldLocate\": false}}";
        private readonly static String WIPE_DEVICE_TMPL = "{\"device\": \"%s\", \"serverContext\": {\"maxLocatingTime\": 90000, \"deviceLoadStatus\": \"203\", \"prefsUpdateTime\": 1276872996660, \"lastSessionExtensionTime\": null, \"clientId\": \"0000000000000000000000000000000000000000\", \"timezone\": {\"previousOffset\": -28800000, \"currentOffset\": -25200000, \"previousTransition\": 1268560799999, \"tzCurrentName\": \"Pacific Daylight Time\", \"tzName\": \"America/Los_Angeles\"}, \"validRegion\": true, \"sessionLifespan\": 900000, \"preferredLanguage\": \"en\", \"hasDevices\": true, \"maxDeviceLoadTime\": 60000, \"callbackIntervalInMS\": 3000}, \"clientContext\": {\"deviceUDID\": \"0000000000000000000000000000000000000000\", \"selectedDevice\": \"%s\", \"inactiveTime\": 5911, \"productType\": \"iPad1,1\", \"appName\": \"FindMyiPhone\", \"buildVersion\": \"145\", \"osVersion\": \"3.2\", \"appVersion\": \"1.4\", \"shouldLocate\": false}}";

        private Commands()
        {
            // no instantiation
        }

        public static String GetClientInitContext()
        {
            return INIT_CLIENT;
        }

        public static String GetSendMsgCmd(bool playSound, String message, String title, Device device)
        {
            String msg = message ?? string.Empty;
            String subject = title ?? "Important Message";
            if (device == null)
            {
                throw new ArgumentException("Device should not be null");
            }
            if (device.Id == null)
            {
                throw new ArgumentException("Device Id should never be null.");
            }
            return string.Format(SEND_MESSAGE_TMPL, playSound, msg, device.Id, device.Id, subject);
        }

        public static String GetLockDeviceCmd(String passcode, Device device)
        {
            String newPasscode = passcode ?? string.Empty;
            if (device == null)
            {
                throw new ArgumentException("Device should not be null");
            }
            if (device.Id == null)
            {
                throw new ArgumentException("Device Id should never be null.");
            }
            return string.Format(LOCK_DEVICE_TMPL, device.Id, newPasscode, device.Id);
        }

        public static String GetWipeDeviceCmd(Device device)
        {
            if (device == null)
            {
                throw new ArgumentException("Device should not be null");
            }
            if (device.Id == null)
            {
                throw new ArgumentException("Device Id should never be null.");
            }
            return string.Format(WIPE_DEVICE_TMPL, device.Id, device.Id);
        }
    }
}
