using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using FindMyIphoneSharp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FindMyIphoneSharp
{
    public class DeviceLocator
    {
        private static readonly int DEFAULT_LOCATE_TIMEOUT = 300; // in seconds
        private readonly String _userName;
        private readonly String _password;
        private readonly List<Device> _devices = new List<Device>();
        private String _partition;
        private UserInfo _userInfo;

        private const String _initTmpl = "/fmipservice/device/{0}/initClient";

        public DeviceLocator(String userName, String password)
        {
            this._userName = userName;
            this._password = password;
            Init();
        }

        private void Init()
        {
            var response = Post(string.Format(_initTmpl, _userName), Commands.GetClientInitContext());
            _partition = response.Headers.Get("X-Apple-MMe-Host");
            UpdateDevices();
        }

        private void UpdateDevices()
        {
            try
            {
                var response = Post(string.Format(_initTmpl, _userName), Commands.GetClientInitContext());
                var text = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var json = (JObject)JsonConvert.DeserializeObject(text);
                //if (json.containsKey("error"))
                //{
                //    throw new InvalidResponseException((String)json["error"));
                //}
                JObject userInfoObj = (JObject)json["userInfo"];
                _userInfo = new UserInfo((String)userInfoObj["firstName"], (String)userInfoObj["lastName"]);

                JArray content = (JArray)json["content"];
                _devices.Clear();
                foreach (JObject x in content)
                {
                    Device device = new Device();
                    PopulateProps(device, x);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error parsing server response.", e);
            }
        }

        public void Reinit()
        {
            Init();
        }

        private void PopulateProps(Device device, JObject deviceProps)
        {
            device.CanWipeAfterLock = ((Boolean)deviceProps["canWipeAfterLock"]);
            device.LocFoundEnabled = ((Boolean)deviceProps["locFoundEnabled"]);
            JObject loc = (JObject)deviceProps["location"];
            if (loc != null)
            {
                LocationInfo deviceLocationInfo = new LocationInfo();
                deviceLocationInfo.TimeStamp = ((long)loc["timeStamp"]);
                deviceLocationInfo.LocationType = (loc["locationType"]);
                deviceLocationInfo.IsLocationFinished = ((Boolean)loc["locationFinished"]);
                deviceLocationInfo.HorizontalAccuracy = ((Double)loc["horizontalAccuracy"]);
                deviceLocationInfo.PositionType = ((String)loc["positionType"]);
                deviceLocationInfo.Longitude = ((Double)loc["longitude"]);
                deviceLocationInfo.IsInaccurate = ((Boolean)loc["isInaccurate"]);
                deviceLocationInfo.Latitude = ((Double)loc["latitude"]);
                deviceLocationInfo.IsOld = ((Boolean)loc["isOld"]);
                device.LocationInfo = (deviceLocationInfo);
            }
            device.DeviceModel = ((String)deviceProps["deviceModel"]);
            JObject remLck = (JObject)deviceProps["remoteLock"];
            if (remLck != null)
            {
                RemoteLockInfo remoteLockInfo = new RemoteLockInfo
                {
                    StatusCode = ((String)remLck["statusCode"]),
                    CreateTimestamp = ((long)remLck["createTimestamp"])
                };
                device.RemoteLockInfo = (remoteLockInfo);
            }
            device.ActivationLocked = ((Boolean)deviceProps["activationLocked"]);
            device.RawDeviceModel = ((String)deviceProps["rawDeviceModel"]);
            device.LocationEnabled = ((Boolean)deviceProps["locationEnabled"]);
            device.ModelDisplayName = ((String)deviceProps["modelDisplayName"]);
            device.Id = ((String)deviceProps["id"]);
            device.LostModeCapable = ((Boolean)deviceProps["lostModeCapable"]);
            device.DeviceDisplayName = ((String)deviceProps["deviceDisplayName"]);
            device.DarkWake = ((Boolean)deviceProps["darkWake"]);
            device.LocationCapable = ((Boolean)deviceProps["locationCapable"]);
            device.MaxMsgChar = ((long)deviceProps["maxMsgChar"]);
            device.Name = ((String)deviceProps["name"]);
            device.BatteryLevel = ((Double)deviceProps["batteryLevel"]);
            JObject f = (JObject)deviceProps["features"];
            if (f != null)
            {
                FeaturesInfo featuresInfo = new FeaturesInfo();
                featuresInfo.IsCltEnabled = ((Boolean)f["CLT"]);
                featuresInfo.IsCwpEnabled = ((Boolean)f["CWP"]);
                featuresInfo.IsWmgEnabled = ((Boolean)f["WMG"]);
                featuresInfo.IsXrmEnabled = ((Boolean)f["XRM"]);
                featuresInfo.IsClkEnabled = ((Boolean)f["CLK"]);
                featuresInfo.IsSndEnabled = ((Boolean)f["SND"]);
                featuresInfo.IsLstEnabled = ((Boolean)f["LST"]);
                featuresInfo.IsKeyEnabled = ((Boolean)f["KEY"]);
                featuresInfo.IsWipEnabled = ((Boolean)f["WIP"]);
                featuresInfo.IsLocEnabled = ((Boolean)f["LOC"]);
                featuresInfo.IsLlcEnabled = ((Boolean)f["LLC"]);
                featuresInfo.IsMsgEnabled = ((Boolean)f["MSG"]);
                featuresInfo.IsLmgEnabled = ((Boolean)f["LMG"]);
                featuresInfo.IsRemEnabled = ((Boolean)f["REM"]);
                featuresInfo.IsLckEnabled = ((Boolean)f["LCK"]);
                featuresInfo.IsSvpEnabled = ((Boolean)f["SVP"]);
                featuresInfo.IsLklEnabled = ((Boolean)f["LKL"]);
                featuresInfo.IsTeuEnabled = ((Boolean)f["TEU"]);
                featuresInfo.IsPinEnabled = ((Boolean)f["PIN"]);
                featuresInfo.IsLkmEnabled = ((Boolean)f["LKM"]);
                featuresInfo.IsKpdEnabled = ((Boolean)f["KPD"]);
                device.FeaturesInfo = (featuresInfo);
            }
            device.DeviceClass = ((String)deviceProps["deviceClass"]);
            device.WipeInProgress = ((Boolean)deviceProps["wipeInProgress"]);
            device.PasscodeLength = ((long)deviceProps["passcodeLength"]);
            device.IsMac = ((Boolean)deviceProps["isMac"]);
            device.IsLocating = ((Boolean)deviceProps["isLocating"]);
            device.DeviceColor = ((String)deviceProps["deviceColor"]);
            device.BatteryStatus = ((String)deviceProps["batteryStatus"]);
            device.DeviceStatus = ((String)deviceProps["deviceStatus"]);
            device.LockedTimestamp = ((long)deviceProps["lockedTimestamp"]);
            device.WipedTimestamp = ((long)deviceProps["wipedTimestamp"]);
            JObject msgObj = (JObject)deviceProps["msg"];
            if (msgObj != null)
            {
                MsgInfo msgInfo = new MsgInfo();
                msgInfo.StatusCode = ((String)msgObj["statusCode"]);
                msgInfo.CreateTimestamp = ((long)msgObj["createTimestamp"]);
                msgInfo.UserText = ((Boolean)msgObj["userText"]);
                msgInfo.PlaySound = ((Boolean)msgObj["playSound"]);
                device.MsgInfo = msgInfo;
            }
            device.LostModeEnabled = ((Boolean)deviceProps["lostModeEnabled"]);
            device.LostTimestamp = ((String)deviceProps["lostTimestamp"]);
            JObject lstDev = (JObject)deviceProps["lostDevice"];
            if (lstDev != null)
            {
                LostDeviceInfo lostDeviceInfo = new LostDeviceInfo();
                lostDeviceInfo.StatusCode = ((String)lstDev["statusCode"]);
                lostDeviceInfo.CreateTimestamp = ((long)lstDev["createTimestamp"]);
                lostDeviceInfo.Text = ((String)lstDev["text"]);
                lostDeviceInfo.UserText = ((Boolean)lstDev["userText"]);
                lostDeviceInfo.StopLostMode = ((Boolean)lstDev["stopLostMode"]);
                lostDeviceInfo.OwnerNbr = ((String)lstDev["ownerNbr"]);
                lostDeviceInfo.Sound = ((Boolean)lstDev["sound"]);
                lostDeviceInfo.EmailUpdates = ((Boolean)lstDev["emailUpdates"]);
                device.LostDeviceInfo = (lostDeviceInfo);
            }
            device.UserInfo = _userInfo;
        }

        public ReadOnlyCollection<Device> GetDevices()
        {
            return _devices.AsReadOnly();
        }

        public Device Locate(Device device)
        {
            return Locate(device, DEFAULT_LOCATE_TIMEOUT);
        }

        public Device Locate(Device device, int timeout)
        {
            if (device == null)
            {
                throw new ArgumentNullException("Device should not be null.");
            }
            if (timeout < 0)
            {
                throw new ArgumentException("Timeout cannot be a negative value.");
            }
            String deviceId = device.Id;
            long start = DateTime.Now.Ticks;
            long timeoutMillis = timeout;
            Device deviceToLocate = device;
            while (!deviceToLocate.LocationInfo.IsLocationFinished)
            {
                if (DateTime.Now.Ticks - start > timeoutMillis)
                {
                    throw new LocateTimeoutException(timeout);
                }
                Thread.Sleep(5);
                UpdateDevices();
                deviceToLocate = FindDevice(deviceId);
                if (deviceToLocate == null)
                {
                    throw new DeviceNotFoundException(deviceId);
                }
            }
            return deviceToLocate;
        }

        private string GetBase64String(string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }

        private HttpWebResponse Post(String url, String body)
        {
            String fullUrl = string.Format("https://{0}{1}", (_partition ?? "fmipmobile.icloud.com"), url);
            HttpWebRequest request = WebRequest.CreateHttp(fullUrl);

            try
            {
                request.Headers.Add("Content-type", "application/json; charset=utf-8");
                request.Headers.Add("X-apple-find-api-ver", "2.0");
                request.Headers.Add("X-apple-authscheme", "UserIdGuest");
                request.Headers.Add("X-apple-realm-support", "1.0");
                request.Headers.Add("User-agent", "Find iPhone/1.2 MeKit (iPad: iPhone OS/4.2.1)");
                request.Headers.Add("X-client-name", "iPad");
                request.Headers.Add("X-client-uuid", "0cf3dc501ff812adb0b202baed4f37274b210853");
                request.Headers.Add("Accept-language", "en-us");
                request.Headers.Add("Connection", "keep-alive");
                String credentials = string.Format("{0}:{1}", _userName, _password);
                String encodedAuth = GetBase64String(credentials);
                request.Headers.Add("Authorization", "Basic " + encodedAuth);
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);

                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                byte[] postData = Encoding.UTF8.GetBytes(body);//postDataStr即为发送的数据，格式还是和上次说的一样 
                request.ContentLength = postData.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);

                var response = (HttpWebResponse)request.GetResponse();

                //int responseCode = response.StatusCode;
                //String responseText = connection.getResponseMessage();
                ////ignores 330 error.
                ////330 error raises when redirected response did not contains
                ////any content. (expected: 204 No Contents)
                //if (responseCode == 330)
                //{
                //    responseCode = 204;
                //}

                //if (responseCode < 200 || responseCode >= 300)
                //{
                //    throw new InvalidResponseException(responseText);
                //}

                return response;
            }
            catch (IOException e)
            {
                //if (connection != null)
                //{
                //    connection.disconnect();
                //}
                throw;
            }
        }

        public void SendMessage(Device device, String msg, String title, bool playSound)
        {
            String cmd = Commands.GetSendMsgCmd(playSound, msg, title, device);
            Post(string.Format("/fmipservice/device/{0}/sendMessage", _userName), cmd);
        }

        public void LockDevice(Device device, String passcode)
        {
            String cmd = Commands.GetLockDeviceCmd(passcode, device);
            Post(string.Format("/fmipservice/device/{0}/remoteLock", _userName), cmd);
        }

        public void WipeDevice(Device device)
        {
            String cmd = Commands.GetWipeDeviceCmd(device);
            Post(string.Format("/fmipservice/device/{0}/remoteWipe", _userName), cmd);
        }

        private Device FindDevice(String id)
        {
            return _devices.FirstOrDefault(d => d.Id == id);
        }
    }

    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException(string deviceId)
        {
        }
    }

    public class LocateTimeoutException : Exception
    {
        public LocateTimeoutException(int timeout)
        {
        }
    }
}
