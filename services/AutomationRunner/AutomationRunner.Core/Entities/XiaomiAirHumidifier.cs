using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
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
        public enum Name
        {
            [EntityId("fan.air_humidifier")]
            AirHumidifier
        }

        public int Humidity => GetAttributeValue<int>(nameof(Humidity));
        public int Temperature => GetAttributeValue<int>(nameof(Temperature));
        public int? Depth => GetAttributeValue<int>(nameof(Depth));
        public AirHumidifierSpeed Speed => GetAttributeValue<AirHumidifierSpeed>(nameof(Speed));

        public static async Task<XiaomiAirHumidifier> LoadFromEntityId(HomeAssistantConnector entityLoader, Name entityId)
        {
            return await entityLoader.LoadEntityFromStates<XiaomiAirHumidifier>(entityId.GetEntityId());
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

        public async Task SetSpeed(AirHumidifierSpeed speed)
        {
            await Connector.SendService("fan.set_speed", new SetPresetModeService(EntityId, speed.ToString()));
        }
    }
}
