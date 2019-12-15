using System;

namespace AutomationRunner.Core.Common
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
    }

    public class DateTimeHelper : IDateTimeHelper
    {
        public virtual DateTime Now => DateTime.Now;
    }
}
