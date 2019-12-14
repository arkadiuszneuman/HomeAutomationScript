using Newtonsoft.Json;

namespace AutomationRunner.Entities.Services.Models
{
    public class EntityIdService : IService
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }

        public EntityIdService()
        {
        }

        public EntityIdService(string entityId)
        {
            EntityId = entityId;
        }
    }
}
