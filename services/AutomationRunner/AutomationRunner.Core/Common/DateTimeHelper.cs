using System;
using TimeZoneConverter;

namespace AutomationRunner.Core.Common
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime Now
        {
            get
            {
                var tzi = TZConvert.GetTimeZoneInfo("Central European Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(UtcNow, tzi);
            }
        }

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
