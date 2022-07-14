using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class InputSelect : BaseEntity
    {
        public enum Name
        {
            [EntityId("input_select.air_purifier_2s_automation_type")]
            AirPurifier2sAutomationType
        }

        public static InputSelect CreateBasedOnBaseEntity(BaseEntity oldState)
        {
            var entity = new InputSelect();
            entity.UpdateEntity(oldState);
            return entity;
        }

        public static async Task<InputSelect> LoadFromEntityId(HomeAssistantConnector connector, Name inputNumberName) =>
            await connector.LoadEntityFromStates<InputSelect>(inputNumberName.GetEntityId());

        public async Task SelectOption(string option) => 
            await Connector.SendService("input_select.select_option", new OptionServiceModel(EntityId, option));
    }
}
