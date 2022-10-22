using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using FluentAssertions;
using NSubstitute;
using System;
using Xunit;

namespace AutomationRunner.UnitTests.Automations.Helpers
{
    public class ConditionHelperTests
    {
        [Fact]
        public void Return_true_if_condition_is_fulfilled_for_set_time()
        {
            var dateTimeHelper = Substitute.For<IDateTimeHelper>();
            var now = new DateTime(2019, 3, 6, 12, 32, 42);
            dateTimeHelper.Now.Returns(x => now);

            var sut = new ConditionHelper(dateTimeHelper)
                .For(TimeSpan.FromMinutes(5));


            sut.CheckFulfilled(true).Should().BeFalse();
            now += TimeSpan.FromSeconds(10);
            sut.CheckFulfilled(false).Should().BeFalse();
            now += TimeSpan.FromMinutes(3);
            sut.CheckFulfilled(true).Should().BeFalse();
            now += TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(1);
            sut.CheckFulfilled(true).Should().BeFalse();
            now += TimeSpan.FromSeconds(1);
            sut.CheckFulfilled(true).Should().BeTrue();
        }
    }
}
