using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FindMyIphoneSharp
{
    class Program
    {
        static void Main(string[] args)
        {

            var locator = new DeviceLocator(File.ReadAllText("user"), File.ReadAllText("pass"));
            var devices = locator.GetDevices().ToList();

            var device = devices[0];

            while (true)
            {
                Console.WriteLine("{0},{1}", device.LocationInfo.Longitude, device.LocationInfo.Latitude);
                Console.WriteLine("Battery :{0} left at {1}", device.BatteryLevel, device.LocationInfo.TimeStamp);
                device.LocationInfo.IsLocationFinished = false;
                try
                {
                    locator.Locate(device);
                }
                catch (LocateTimeoutException)
                {
                }
                Thread.Sleep(6000);
            }

            Console.ReadKey();
        }
    }
}
