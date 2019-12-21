using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class Cover : BaseEntity
    {
        public enum Name
        {
            [EntityId("cover.roleta_salon")]
            Salon
        }

        public static async Task<Cover> LoadFromEntityId(HomeAssistantConnector entityLoader, Name coverName)
        {
            return await entityLoader.LoadEntityFromStates<Cover>(coverName.GetEntityId());
        }

        public async Task OpenCover()
        {
            await Connector.SendService("cover.open_cover", new EntityIdService(EntityId));
        }

        public async Task CloseCover()
        {
            await Connector.SendService("cover.close_cover", new EntityIdService(EntityId));
        }
    }
}
