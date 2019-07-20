using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    public class StairsParametersHandler : ISubscribedTopic
    {
        public string TopicName => "home/stairs/parameters/set";

        public Task Handle(string payload)
        {
            Console.WriteLine(payload);
            return Task.CompletedTask;
        }
    }
}
