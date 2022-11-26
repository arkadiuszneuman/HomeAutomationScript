using AutomationRunner.Core.Common.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationRunner.Core.Entities
{
    public record BaseEntity
    {
        public HomeAssistantConnector Connector { get; set; }

        [JsonProperty("entity_id")] 
        public string EntityId { get; set; } = string.Empty;
        public string State { get; set; } = String.Empty;
        public Dictionary<string, object> Attributes { get; private set; } = new();

        protected T? GetAttributeValue<T>(string name)
        {
            if (!Attributes.ContainsKey(name.ToLower()))
                return default;

            var attributeValue = Attributes[name.ToLower()];

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), attributeValue.ToString() ?? string.Empty);
            }

            return (T)Convert.ChangeType(attributeValue, typeof(T));
        }

        public virtual void UpdateEntity<T>(T entity)
            where T : BaseEntity
        {
            EntityId = entity.EntityId;
            State = entity.State;
            Attributes = entity.Attributes;
            Connector = entity.Connector;
        }
    }
}
