using System;

namespace FindMyIphoneSharp
{
    public static class DateTimeUtils
    {
        public static DateTime FromTimeStamp(long timeStamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(timeStamp / 1000).ToLocalTime();
        }

        public static DateTime ToDateTime(this long timeStamp)
        {
            return FromTimeStamp(timeStamp);
        }

        public static DateTime? ToDateTime(this long? timeStamp)
        {
            return timeStamp.HasValue ? FromTimeStamp(timeStamp.Value) : (DateTime?)null;
        }
    }
}
