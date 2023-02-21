using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public record Cover : BaseEntity
    {
        public enum Name
        {
            [EntityId("cover.roleta_salon")]
            Salon
        }

        public int Position => GetAttributeValue<int>("current_position");

        public static async Task<Cover> LoadFromEntityId(HomeAssistantConnector entityLoader, Name coverName)
        {
            return await entityLoader.LoadEntityFromStates<Cover>(coverName.GetEntityId());
        }

        public async Task OpenCover()
        {
            await Connector.SendServiceAsync("cover.open_cover", new EntityIdService(EntityId));
        }

        public async Task CloseCover()
        {
            await Connector.SendServiceAsync("cover.close_cover", new EntityIdService(EntityId));
        }

        public async Task SetCoverPosition(int position)
        {
            await Connector.SendServiceAsync("cover.set_cover_position", new CoverPositionServiceModel(EntityId, position));
        }
    }
}
