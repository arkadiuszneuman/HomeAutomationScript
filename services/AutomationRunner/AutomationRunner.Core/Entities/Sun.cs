using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public record Sun : BaseEntity
    {
        public DateTime NextRisingUtc => GetAttributeValue<DateTime>("next_rising");
        public DateTime NextSettingUtc => GetAttributeValue<DateTime>("next_setting");
        public DateTime NextDawnUtc => GetAttributeValue<DateTime>("next_dawn");
        public DateTime NextDuskUtc => GetAttributeValue<DateTime>("next_dusk");
        public DateTime NextMidnightUtc => GetAttributeValue<DateTime>("next_midnight");
        public DateTime NextNoonUtc => GetAttributeValue<DateTime>("next_noon");
        public decimal Elevation => GetAttributeValue<decimal>("elevation");
        public decimal Azimuth => GetAttributeValue<decimal>("azimuth");
        public bool Rising => GetAttributeValue<bool>("rising");

        private IDateTimeHelper DateTimeHelper { get; set; }
        public static new string EntityId => "sun.sun";

        public static async Task<Sun> Load(HomeAssistantConnector connector, IDateTimeHelper dateTimeHelper)
        {
            var sun = await connector.LoadEntityFromStates<Sun>(EntityId);
            sun.DateTimeHelper = dateTimeHelper;
            return sun;
        }

        public bool AfterSunset(TimeSpan offset = default) =>
            DateTimeHelper.UtcNow.Date < (NextSettingUtc + offset).Date
                || DateTimeHelper.UtcNow > NextSettingUtc + offset;
    }
}
