using AutomationRunner.Core.Common;

namespace AutomationRunner.Core.Entities
{
    public class Sensor : BaseEntity
    {
        public enum Name
        {
            [EntityId("sensor.sunlight_pct")]
            Sunlight
        }
    }
}