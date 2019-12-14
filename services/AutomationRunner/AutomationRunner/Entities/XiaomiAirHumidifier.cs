using AutomationRunner.Common.Connector;
using AutomationRunner.Entities.Services;
using AutomationRunner.Entities.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace AutomationRunner.Entities
{
    public enum AirHumidifierSpeed
    {
        High,
        Medium,
        Silent,
        Auto
    }

    public class XiaomiAirHumidifier : BaseEntity
    {
        private HomeAssistantConnector EntityLoader { get; set; }

        public XiaomiAirPurifierAttributes Attributes { get; set; }

        public class XiaomiAirPurifierAttributes
        {
            public int Aqi { get; set; }
            public int Humidity { get; set; }
            public decimal Temperature { get; set; }
            public int Depth { get; set; }

            [JsonProperty("speed")]
            [JsonConverter(typeof(StringEnumConverter))]
            public AirHumidifierSpeed Speed { get; set; }
        }

        public static async Task<XiaomiAirHumidifier> LoadFromEntityId(HomeAssistantConnector entityLoader, string entityId)
        {
            var deserializedObject = await entityLoader.LoadEntityFromStates<XiaomiAirHumidifier>(entityId);
            deserializedObject.EntityLoader = entityLoader;
            return deserializedObject;
        }

        public async Task TurnOn()
        {
            await EntityLoader.SendService("fan.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await EntityLoader.SendService("fan.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SetSpeed(AirHumidifierSpeed speed)
        {
            await EntityLoader.SendService("fan.set_speed", new SetSpeedService(EntityId, speed.ToString()));
        }
    }
}
