using AutomationRunner.Common.Connector;
using System.Threading.Tasks;

namespace AutomationRunner.Entities
{
    public class XiaomiAirPurifier : BaseEntity
    {
        private HomeAssistantConnector EntityLoader { get; set; }

        public XiaomiAirPurifierAttributes Attributes { get; set; }

        public class XiaomiAirPurifierAttributes
        {
            public int Aqi { get; set; }
            public int Humidity { get; set; }
            public decimal Temperature { get; set; }
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
        }

        public async Task TurnOff()
        {
            await EntityLoader.SendService("fan.turn_off", new EntityIdService(EntityId));
        }
    }
}
