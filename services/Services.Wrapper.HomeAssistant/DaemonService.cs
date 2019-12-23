using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.Common;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.MQTT;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics;
using Services.Wrapper.HomeAssistant.RabbitMq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant
{
    public class DaemonService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly RabbitManager _rabbitManager;
        private readonly MqttManager _mqttManager;
        private readonly ProgramStartedInformation _programStartedInformation;

        public DaemonService(ILogger<DaemonService> logger,
            RabbitManager rabbitManager,
            MqttManager mqttManager,
            ProgramStartedInformation programStartedInformation)
        {
            _logger = logger;
            _rabbitManager = rabbitManager;
            _mqttManager = mqttManager;
            _programStartedInformation = programStartedInformation;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _programStartedInformation.DateTime = DateTime.Now;

            await _mqttManager.ConnectAsync();
            await _mqttManager.Publish<WindowCoverPositionTopic>(c => "100");
            await _mqttManager.AddHandler<MinLevelModel>();
            await _mqttManager.AddHandler<MaxLevelModel>();
            await _mqttManager.AddHandler<WindowCoverStateModel>();
            await _mqttManager.AddHandler<WindowCoverSetPositionModel>();

            _rabbitManager
                .Connect()
                .AddHandler<TriggeredUpperStairSensorModel>()
                .AddHandler<TriggeredBottomStairSensorModel>()
                .AddHandler<CoverLevelChangedModel>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
