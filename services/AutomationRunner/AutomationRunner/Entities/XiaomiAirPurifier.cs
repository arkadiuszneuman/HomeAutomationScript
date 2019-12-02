using AutomationRunner.Common.EntityLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationRunner.Entities
{
    public class XiaomiAirPurifier : BaseEntity
    {
        private EntityLoader EntityLoader { get; set; }

        public XiaomiAirPurifierAttributes Attributes { get; set; }

        public class XiaomiAirPurifierAttributes
        {
            public int Aqi { get; set; }
            public int Humidity { get; set; }
            public decimal Temperature { get; set; }
        }

        public static async Task<XiaomiAirPurifier> LoadFromEntityId(EntityLoader entityLoader, string entityId)
        {
            var deserializedObject = await entityLoader.LoadEntityFromStates<XiaomiAirPurifier>(entityId);
            deserializedObject.EntityLoader = entityLoader;
            return deserializedObject;
        }
    }
}
