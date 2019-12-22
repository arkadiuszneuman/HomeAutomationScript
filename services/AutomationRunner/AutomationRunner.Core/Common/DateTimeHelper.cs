using System;

namespace AutomationRunner.Core.Common
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
