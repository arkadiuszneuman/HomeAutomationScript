using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics
{
    public class BottomMotionDetectorTopic : ITopic
    {
        public string TopicName => "home/stairs/bottom_motion_detector/set";
        public string ON => "ON";
        public string OFF => "Off";
    }
}
