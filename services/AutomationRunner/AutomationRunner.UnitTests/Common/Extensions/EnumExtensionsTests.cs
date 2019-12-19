using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AutomationRunner.UnitTests.Common.Extensions
{
    public class EnumExtensionsTests
    {
        [Theory]
        [InlineData(Light.Name.Halogen1, "light.halogen_1")]
        [InlineData(Light.Name.Halogen2, "light.halogen_2")]
        [InlineData(Light.Name.Halogen3, "light.halogen_3")]
        [InlineData(Light.Name.Halogen4, "light.halogen_4")]
        [InlineData(Light.Name.ExternalLight, "light.lampka_zewnetrzna")]
        public void Return_entity_id_from_attribute(Light.Name enumValue, string expectedResult)
        {
            var result = enumValue.GetEntityId();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Return_defult_when_attribute_does_not_exist()
        {
            var name = AirHumidifierSpeed.High;

            var result = name.GetEntityId();

            result.Should().Be("High");
        }
    }
}
