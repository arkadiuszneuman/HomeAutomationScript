using AutomationRunner.Common.Connector;
using AutomationRunner.Entities.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace AutomationRunner.Entities
{
    public enum AirPurifierSpeed
    {
        Auto,
        Favorite,
        Silent
    }

    public class XiaomiAirPurifier : BaseEntity
    {
        private HomeAssistantConnector EntityLoader { get; set; }

        public XiaomiAirPurifierAttributes Attributes { get; set; }

        public class XiaomiAirPurifierAttributes
        {
            public int Aqi { get; set; }
            public int Humidity { get; set; }
            public decimal Temperature { get; set; }

            [JsonProperty("favorite_level")]
            public int FavoriteLevel { get; set; }

            [JsonProperty("speed")]
            [JsonConverter(typeof(StringEnumConverter))]
            public AirPurifierSpeed Speed { get; set; }
        }

        public static async Task<XiaomiAirPurifier> LoadFromEntityId(HomeAssistantConnector entityLoader, string entityId)
        {
            var deserializedObject = await entityLoader.LoadEntityFromStates<XiaomiAirPurifier>(entityId);
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

        public async Task SetSpeed(AirPurifierSpeed speed)
        {
            await EntityLoader.SendService("fan.set_speed", new SetSpeedService(EntityId, speed.ToString()));
        }

        public async Task SetLevel(int level)
        {
            await EntityLoader.SendService("fan.xiaomi_miio_set_favorite_level", 
                new XiaomiMiioSetFavoriteLevelService(EntityId, level));
        }
    }
}
