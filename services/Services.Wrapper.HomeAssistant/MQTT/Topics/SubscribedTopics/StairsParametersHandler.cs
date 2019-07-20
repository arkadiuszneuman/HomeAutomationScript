using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    public class MinLevelModel
    {
        public string Level { get; set; }
    }

    public class StairsParametersHandler : ISubscribedTopic<MinLevelModel>
    {
        public string TopicName => "home/stairs/parameters/set";

        public Task Handle(MinLevelModel payload)
        {
            Console.WriteLine(payload);
            return Task.CompletedTask;
        }
    }
}
