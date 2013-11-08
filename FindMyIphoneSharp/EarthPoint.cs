using System;

namespace FindMyIphoneSharp
{
    public struct EarthPoint
    {
        public const double Ea = 6378137; // 赤道半径 WGS84标准参考椭球中的地球长半径(单位:m) 
        public const double Eb = 6356725; // 极半径  
        public readonly double Longitude, Latidute;
        public readonly double Jd;
        public readonly double Wd;
        public readonly double Ec;
        public readonly double Ed;

        public EarthPoint(double longitude, double latidute)
        {
            Longitude = longitude;
            Latidute = latidute;
            Jd = Longitude * Math.PI / 180; //转换成角度 
            Wd = Latidute * Math.PI / 180; //转换成角度 
            Ec = Eb + (Ea - Eb) * (90 - Latidute) / 90;
            Ed = Ec * Math.Cos(Wd);
        }

        public double Distance(EarthPoint point)
        {
            double dx = (point.Jd - Jd) * Ed;
            double dy = (point.Wd - Wd) * Ec;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistance(double longitude1, double latidute1, double longitude2, double latidute2)
        {
            EarthPoint p1 = new EarthPoint(longitude1, latidute1);
            EarthPoint p2 = new EarthPoint(longitude2, latidute2);
            return p1.Distance(p2);
        }
    }
}