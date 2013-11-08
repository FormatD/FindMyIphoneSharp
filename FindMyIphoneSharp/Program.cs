using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FindMyIphoneSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            int sleepTimeTenSeconds = 30;
            const string outPath = "result.txt";
            var locator = new DeviceLocator(File.ReadAllText("user"), File.ReadAllText("pass"));
            var devices = locator.GetDevices().ToList();

            var device = devices[0];

            ThreadPool.QueueUserWorkItem((o) =>
            {
                while (true)
                {
                    int temp;
                    if (int.TryParse(Console.ReadLine(), out temp))
                        sleepTimeTenSeconds = temp;
                }
            });

            while (true)
            {
                Console.WriteLine("{0},{1}", device.LocationInfo.Latitude, device.LocationInfo.Longitude);
                Console.WriteLine("Battery :{0} left at {1}", device.BatteryLevel, DateTime.Now);
                File.AppendAllText(outPath, string.Format("{0}\t{1},{2}\t{3}\t{4}\n", DateTime.Now, device.LocationInfo.Latitude, device.LocationInfo.Longitude, device.LocationInfo.LocationType, device.BatteryLevel));
                device.LocationInfo.IsLocationFinished = false;
                try
                {
                    device = locator.Locate(device);
                }
                catch (LocateTimeoutException)
                {
                }
                Thread.Sleep(sleepTimeTenSeconds * 10000);
            }
        }
    }
}
