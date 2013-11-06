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
        private const int _defaultLocateTimeout = 300; // in seconds
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
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return;

                var text = new StreamReader(responseStream).ReadToEnd();
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
                    _devices.Add(device);
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
            try
            {
                device.CanWipeAfterLock = ((Boolean)deviceProps["canWipeAfterLock"]);
                device.LocFoundEnabled = ((Boolean)deviceProps["locFoundEnabled"]);
                JObject loc = deviceProps["location"] as JObject;
                if (loc != null)
                {
                    LocationInfo deviceLocationInfo = new LocationInfo
                    {
                        TimeStamp = (long)loc["timeStamp"],
                        LocationType = loc["locationType"],
                        IsLocationFinished = (Boolean)loc["locationFinished"],
                        HorizontalAccuracy = (Double)loc["horizontalAccuracy"],
                        PositionType = (String)loc["positionType"],
                        Longitude = (Double)loc["longitude"],
                        IsInaccurate = (Boolean)loc["isInaccurate"],
                        Latitude = (Double)loc["latitude"],
                        IsOld = (Boolean)loc["isOld"]
                    };
                    device.LocationInfo = deviceLocationInfo;
                }
                device.DeviceModel = ((String)deviceProps["deviceModel"]);
                JObject remLck = deviceProps["remoteLock"] as JObject;
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
                JObject f = deviceProps["features"] as JObject;
                if (f != null)
                {
                    FeaturesInfo featuresInfo = new FeaturesInfo
                    {
                        IsCltEnabled = ((Boolean)f["CLT"]),
                        IsCwpEnabled = ((Boolean)f["CWP"]),
                        IsWmgEnabled = ((Boolean)f["WMG"]),
                        IsXrmEnabled = ((Boolean)f["XRM"]),
                        IsClkEnabled = ((Boolean)f["CLK"]),
                        IsSndEnabled = ((Boolean)f["SND"]),
                        IsLstEnabled = ((Boolean)f["LST"]),
                        IsKeyEnabled = ((Boolean)f["KEY"]),
                        IsWipEnabled = ((Boolean)f["WIP"]),
                        IsLocEnabled = ((Boolean)f["LOC"]),
                        IsLlcEnabled = ((Boolean)f["LLC"]),
                        IsMsgEnabled = ((Boolean)f["MSG"]),
                        IsLmgEnabled = ((Boolean)f["LMG"]),
                        IsRemEnabled = ((Boolean)f["REM"]),
                        IsLckEnabled = ((Boolean)f["LCK"]),
                        IsSvpEnabled = ((Boolean)f["SVP"]),
                        IsLklEnabled = ((Boolean)f["LKL"]),
                        IsTeuEnabled = ((Boolean)f["TEU"]),
                        IsPinEnabled = ((Boolean)f["PIN"]),
                        IsLkmEnabled = ((Boolean)f["LKM"]),
                        IsKpdEnabled = ((Boolean)f["KPD"])
                    };
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
                device.LockedTimestamp = ((long?)deviceProps["lockedTimestamp"]);
                device.WipedTimestamp = ((long?)deviceProps["wipedTimestamp"]);
                JObject msgObj = deviceProps["msg"] as JObject;
                if (msgObj != null)
                {
                    MsgInfo msgInfo = new MsgInfo
                    {
                        StatusCode = ((String)msgObj["statusCode"]),
                        CreateTimestamp = ((long)msgObj["createTimestamp"]),
                        UserText = ((Boolean)msgObj["userText"]),
                        PlaySound = ((Boolean)msgObj["playSound"])
                    };
                    device.MsgInfo = msgInfo;
                }
                device.LostModeEnabled = ((Boolean)deviceProps["lostModeEnabled"]);
                device.LostTimestamp = ((String)deviceProps["lostTimestamp"]);
                JObject lstDev = deviceProps["lostDevice"] as JObject;
                if (lstDev != null)
                {
                    LostDeviceInfo lostDeviceInfo = new LostDeviceInfo
                    {
                        StatusCode = ((String)lstDev["statusCode"]),
                        CreateTimestamp = ((long)lstDev["createTimestamp"]),
                        Text = ((String)lstDev["text"]),
                        UserText = ((Boolean)lstDev["userText"]),
                        StopLostMode = ((Boolean)lstDev["stopLostMode"]),
                        OwnerNbr = ((String)lstDev["ownerNbr"]),
                        Sound = ((Boolean)lstDev["sound"]),
                        EmailUpdates = ((Boolean)lstDev["emailUpdates"])
                    };
                    device.LostDeviceInfo = (lostDeviceInfo);
                }
                device.UserInfo = _userInfo;
            }
            catch (Exception)
            {

            }
        }

        public ReadOnlyCollection<Device> GetDevices()
        {
            return _devices.AsReadOnly();
        }

        public Device Locate(Device device)
        {
            return Locate(device, _defaultLocateTimeout);
        }

        public Device Locate(Device device, int timeout)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
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

            request.ContentType = "application/json; charset=utf-8";
            request.UserAgent = "Find iPhone/1.2 MeKit (iPad: iPhone OS/4.2.1)";
            request.KeepAlive = true;

            request.Headers.Add("X-apple-find-api-ver", "2.0");
            request.Headers.Add("X-apple-authscheme", "UserIdGuest");
            request.Headers.Add("X-apple-realm-support", "1.0");
            request.Headers.Add("X-client-name", "iPad");
            request.Headers.Add("X-client-uuid", "0cf3dc501ff812adb0b202baed4f37274b210853");
            request.Headers.Add("Accept-language", "en-us");
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

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch (WebException ex)
            {
                return ex.Response as HttpWebResponse;
            }
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
            DeviceId = deviceId;
        }

        public string DeviceId
        {
            get;
            set;
        }
    }

    public class LocateTimeoutException : Exception
    {
        public int Timeout { get; set; }

        public LocateTimeoutException(int timeout)
        {
            Timeout = timeout;
        }
    }
}
