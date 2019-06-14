using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Helper
{
    /// <summary>
    /// DateTime转换帮助类
    /// </summary>
    public class DateTimeHelper
    {
        //DateTime转换为时间戳
        public static long GetTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }

        //timeSpan转换为DateTime
        public static DateTime TimeSpanToDateTime(long span)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }
    }
}
