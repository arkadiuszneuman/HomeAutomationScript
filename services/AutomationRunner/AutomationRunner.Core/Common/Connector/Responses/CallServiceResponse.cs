using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutomationRunner.Core.Common.Connector.Responses
{
    public class CallServiceResponse : Response
    {
        public class EventTypeAttribute
        {
            public class DataAttribute
            {
                [JsonProperty("service_data")]
                public Dictionary<string, object> ServiceData { get; set; } = new Dictionary<string, object>();
                public string Domain { get; internal set; }
                public string Service { get; internal set; }
            }

            public DataAttribute Data { get; set; }
        }

        public EventTypeAttribute Event { get; set; }
    }
}
