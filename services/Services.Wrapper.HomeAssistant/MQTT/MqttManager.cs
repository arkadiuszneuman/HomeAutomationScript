using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Exceptions;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT
{
    public class MqttManager
    {
        private readonly ILogger _logger;
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly IMqttClientFactory _mqttFactory;

        private const string _clientId = "Services.Wrapper.HomeAutomation";
        private readonly IList<ISubscribedTopic> _subscribedTopics = new List<ISubscribedTopic>();
        private IMqttClient _mqttClient;

        public MqttManager(ILogger<MqttManager> logger,
            MqttConfiguration mqttConfiguration,
            IMqttClientFactory mqttFactory)
        {
            _logger = logger;
            _mqttConfiguration = mqttConfiguration;
            _mqttFactory = mqttFactory;
        }

        public async Task ConnectAsync()
        {
            _logger.LogInformation("Connecting to Mosquitto");

            _mqttClient = _mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_mqttConfiguration.Hostname, _mqttConfiguration.Port)
                .Build();

            _mqttClient.UseConnectedHandler(args => _logger.LogInformation("MQTT connected"));
            bool isConnected = false;

            do
            {
                try
                {
                    await _mqttClient.ConnectAsync(options);
                    isConnected = true;
                }
                catch (MqttCommunicationException)
                {
                    _logger.LogWarning("Failed to connect to Mosquitto, reconnecting...");
                    await Task.Delay(2000);
                }
            }
            while (!isConnected);
        }

        public async Task Publish<T>(Func<T, string> predicate)
            where T : ITopic, new()
        {
            if (_mqttClient != null)
            {
                try
                {
                    var topic = Activator.CreateInstance<T>();
                    await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic(topic.TopicName)
                        .WithPayload(predicate(topic))
                        .Build());
                }
                catch (MqttCommunicationException)
                {
                    _logger.LogWarning("Cannot send MQTT message");
                    await ConnectAsync();
                }
            }
        }

        public async Task AddHandler<T>()
            where T : ISubscribedTopic, new()
        {
            if (_mqttClient != null)
            {
                try
                {
                    var topic = Activator.CreateInstance<T>();
                    _subscribedTopics.Add(topic);
                    await _mqttClient.SubscribeAsync(topic.TopicName);
                    _mqttClient.UseApplicationMessageReceivedHandler(HandleReceivedMessages);
                    _logger.LogInformation($"Subscribed {topic.TopicName} topic");
                }
                catch (MqttCommunicationException)
                {
                    _logger.LogWarning("Cannot subscribe to MQTT topic");
                    await ConnectAsync();
                }
            }
        }

        private async Task HandleReceivedMessages(MqttApplicationMessageReceivedEventArgs arg)
        {
            _logger.LogInformation($"Received message to topic {arg.ApplicationMessage.Topic}");
            foreach (var topic in _subscribedTopics.Where(t => t.TopicName == arg.ApplicationMessage.Topic))
            {
                var payload = System.Text.Encoding.Default.GetString(arg.ApplicationMessage.Payload);
                await topic.Handle(payload);
            }
        }
    }
}
