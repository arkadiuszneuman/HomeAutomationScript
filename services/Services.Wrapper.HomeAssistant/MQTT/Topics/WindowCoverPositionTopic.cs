using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics
{
    public class WindowCoverPositionTopic : ITopic
    {
        public string TopicName => "home/cover/position";
    }
}
