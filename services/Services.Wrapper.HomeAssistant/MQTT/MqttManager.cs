using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using System;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT
{
    public class MqttManager
    {
        private readonly ILogger _logger;
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly IMqttClientFactory _mqttFactory;

        private const string _clientId = "Services.Wrapper.HomeAutomation";

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

            _mqttClient.Connected += (s, e) => _logger.LogInformation("MQTT connected");
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
    }
}
