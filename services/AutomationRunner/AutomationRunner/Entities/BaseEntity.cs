using Newtonsoft.Json;

namespace AutomationRunner.Entities
{
    public class BaseEntity
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
        public string State { get; set; }
    }
}
