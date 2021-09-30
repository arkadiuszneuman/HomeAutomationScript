using AutomationRunner.Core.Common.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AutomationRunner.Core.Entities
{
    public class BaseEntity
    {
        public HomeAssistantConnector Connector { get; set; }

        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
        public string State { get; set; }
        public Dictionary<string, object> Attributes { get; set; }

        protected T GetAttributeValue<T>(string name)
        {
            if (!Attributes.ContainsKey(name.ToLower()))
                return default;

            var attributeValue = Attributes[name.ToLower()];

            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), attributeValue.ToString());

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
