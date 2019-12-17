using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AutomationRunner.Core.Entities
{
    public class BaseEntity
    {
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
    }
}
