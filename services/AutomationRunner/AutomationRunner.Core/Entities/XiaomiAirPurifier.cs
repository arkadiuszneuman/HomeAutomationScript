using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public enum AirPurifierSpeed
    {
        Auto,
        Favorite,
        Silent
    }

    public class XiaomiAirPurifier : BaseEntity
    {
        private HomeAssistantConnector Connector { get; set; }

        public int Aqi => GetAttributeValue<int>(nameof(Aqi));
        public int Humidity =>GetAttributeValue<int>(nameof(Humidity));
        public int Temperature => GetAttributeValue<int>(nameof(Temperature));
        public int FavoriteLevel => GetAttributeValue<int>("favorite_level");
        public AirPurifierSpeed Speed => GetAttributeValue<AirPurifierSpeed>(nameof(Speed));

        public static async Task<XiaomiAirPurifier> LoadFromEntityId(HomeAssistantConnector connector, string entityId)
        {
            var deserializedObject = await connector.LoadEntityFromStates<XiaomiAirPurifier>(entityId);
            deserializedObject.Connector = connector;
            return deserializedObject;
        }

        public async Task TurnOn()
        {
            await Connector.SendService("fan.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("fan.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SetSpeed(AirPurifierSpeed speed)
        {
            await Connector.SendService("fan.set_speed", new SetSpeedService(EntityId, speed.ToString()));
        }

        public async Task SetLevel(int level)
        {
            await Connector.SendService("fan.xiaomi_miio_set_favorite_level",
                new XiaomiMiioSetFavoriteLevelService(EntityId, level));
        }
    }
}
