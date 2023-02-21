using Microsoft.Extensions.Logging;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.MQTT;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.Handlers
{
    public class CoverLevelChangedHandler : IHandler<CoverLevelChangedModel>
    {
        private readonly ILogger<CoverLevelChangedHandler> logger;
        private readonly MqttManager mqttManager;

        public CoverLevelChangedHandler(ILogger<CoverLevelChangedHandler> logger,
            MqttManager mqttManager)
        {
            this.logger = logger;
            this.mqttManager = mqttManager;
        }

        public async Task Execute(CoverLevelChangedModel model)
        {
            var level = 100 - model.Level;
            logger.LogInformation("Received cover level from Rabbit - sending to MQTT: {level}", level);
            await mqttManager.Publish<WindowCoverPositionTopic>(c => level.ToString());
        }
    }
}
