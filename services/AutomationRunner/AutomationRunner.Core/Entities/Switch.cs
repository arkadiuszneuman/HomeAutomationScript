using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class Switch : BaseEntity
    {
        public enum Name
        {
            [EntityId("switch.sonoff_1000513cd0")]
            ChildLight,

            [EntityId("switch.sonoff_100051420f")]
            ChristmassTree,

            [EntityId("switch.sonoff_1000511f63")]
            SalonLights,

            [EntityId("switch.sonoff_1000541365")]
            ExternalLight
        }

        public static async Task<Switch> LoadFromEntityId(HomeAssistantConnector entityLoader, Name lightName)
        {
            return await entityLoader.LoadEntityFromStates<Switch>(lightName.GetEntityId());
        }

        public async Task TurnOn()
        {
            await Connector.SendService("switch.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("switch.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}
