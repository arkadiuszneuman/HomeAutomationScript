using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics
{
    public interface ITopic
    {
        string TopicName { get; }
    }
}
