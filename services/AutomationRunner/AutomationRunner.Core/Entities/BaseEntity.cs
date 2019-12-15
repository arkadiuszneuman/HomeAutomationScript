using Newtonsoft.Json;

namespace AutomationRunner.Core.Entities
{
    public class BaseEntity
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
        public string State { get; set; }
    }
}
