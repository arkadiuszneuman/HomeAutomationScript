using AutomationRunner.Common.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AutomationRunner.UnitTests.Common.Extensions
{
    public class DateTimeExtensionsTests
    {
        public static IEnumerable<object[]> Data =>
           new List<object[]>
           {
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 41), new TimeSpan(12, 32, 43), true },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 42), new TimeSpan(12, 32, 43), true },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 42), new TimeSpan(12, 32, 42), true },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 43), new TimeSpan(12, 32, 44), false },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 40), new TimeSpan(12, 32, 41), false },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 42), new TimeSpan(12, 32, 41), true },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(12, 32, 43), new TimeSpan(12, 32, 42), true },
                new object[] { new DateTime(2019, 2, 3, 12, 32, 42), new TimeSpan(23, 0, 0), new TimeSpan(13, 0, 0), true }
           };

        [Theory]
        [MemberData(nameof(Data))]
        public void Checks_if_date_is_between_timespans(DateTime dateTime, TimeSpan from, TimeSpan to, bool expectedResult)
        {
            var result = dateTime.Between(from, to);

            result.Should().Be(expectedResult);
        }
    }
}
