using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    [DebuggerDisplay("MinLevelModel: {Level}")]
    public class MinLevelModel
    {
        public decimal Level { get; set; }
    }

    [DebuggerDisplay("MaxLeveModel: {Level}")]
    public class MaxLevelModel
    {
        public decimal Level { get; set; }
    }

    public class StairsParametersHandler : ISubscribedTopic<MinLevelModel>, ISubscribedTopic<MaxLevelModel>
    {
        private readonly ILogger<StairsParametersHandler> _logger;

        string ISubscribedTopic<MinLevelModel>.TopicName => "home/stairs/minlevel/set";
        string ISubscribedTopic<MaxLevelModel>.TopicName => "home/stairs/maxlevel/set";

        public StairsParametersHandler(ILogger<StairsParametersHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MinLevelModel message)
        {
            _logger.LogInformation("Sending stairs min level {level}", message.Level);
            return Task.CompletedTask;
        }

        public Task Handle(MaxLevelModel message)
        {
            _logger.LogInformation("Sending stairs max level {level}", message.Level);
            return Task.CompletedTask;
        }
    }
}
