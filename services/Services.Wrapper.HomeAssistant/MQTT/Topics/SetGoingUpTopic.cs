using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics
{
    public class SetGoingUpTopic : ITopic
    {
        public string TopicName => "home/stairs/set_going_up";
        public string ON => "ON";
        public string OFF => "Off";
    }
}
