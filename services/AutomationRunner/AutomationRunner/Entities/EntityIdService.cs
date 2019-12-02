using Newtonsoft.Json;

namespace AutomationRunner.Entities
{
    public class EntityIdService
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
