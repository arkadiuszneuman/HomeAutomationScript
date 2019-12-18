using Newtonsoft.Json;

namespace AutomationRunner.Core.Common.Connector.Responses
{
    public class EventTypeResponse : Response
    {
        public class EventTypeAttribute
        {
            [JsonProperty("event_type")]
            public string EventType { get; set; }
        }

        public EventTypeAttribute Event { get; set; }
    }
}
