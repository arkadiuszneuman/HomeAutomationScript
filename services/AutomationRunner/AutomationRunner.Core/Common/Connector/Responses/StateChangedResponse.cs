using AutomationRunner.Core.Entities;
using Newtonsoft.Json;

namespace AutomationRunner.Core.Common.Connector.Responses
{
    public class StateChangedResponse : Response
    {
        public class EventTypeAttribute
        {
            public class DataAttribute
            {
                [JsonProperty("old_state")]
                public BaseEntity OldState { get; set; }

                [JsonProperty("new_state")]
                public BaseEntity NewState { get; set; }
            }

            public DataAttribute Data { get; set; }
        }

        public EventTypeAttribute Event { get; set; }
    }
}
