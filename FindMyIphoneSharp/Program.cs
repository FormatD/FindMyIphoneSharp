using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMyIphoneSharp
{
    class Program
    {
        static void Main(string[] args)
        {

            var locator = new DeviceLocator(File.ReadAllText("user"), File.ReadAllText("pass"));
            var devices = locator.GetDevices().ToList();

            devices[0] = locator.Locate(devices[0]);

            Console.ReadKey();
        }
    }
}
