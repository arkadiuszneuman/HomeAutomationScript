using AutomationRunner.Core.Common;

namespace AutomationRunner.Core.Entities
{
    public class Sensor : BaseEntity
    {
        public enum Name
        {
            [EntityId("sensor.sunlight_pct")]
            Sunlight,
            [EntityId("binary_sensor.laptop_ethernet")]
            LaptopEthernet,
            [EntityId("binary_sensor.laptop_sluzbowy_wifi")]
            BusinessLaptopWifi
        }
    }
}