using System;

namespace AutomationRunner.Core.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool Between(this DateTime dateTime, TimeSpan from, TimeSpan to)
        {
            if (to < from)
            {
                if (dateTime.TimeOfDay >= from)
                    to += TimeSpan.FromDays(1);
                else if (dateTime.TimeOfDay <= to)
                    from -= TimeSpan.FromDays(1);
            }

            var fromDate = AddDate(dateTime, from);
            var toDate = AddDate(dateTime, to);

            return dateTime >= fromDate && dateTime <= toDate;
        }

        private static DateTime AddDate(DateTime dateTime, TimeSpan from)
        {
            return dateTime.Date + from;
        }
    }
}
