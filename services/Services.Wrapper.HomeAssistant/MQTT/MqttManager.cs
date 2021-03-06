﻿using Autofac;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT
{
    public class MqttManager
    {
        private class TopicsContainer
        {
            public string TopicName { get; set; }
            public Func<string, Task> FuncToExecute { get; set; }

            public TopicsContainer(string topicName, Func<string, Task> funcToExecute)
            {
                TopicName = topicName;
                FuncToExecute = funcToExecute;
            }
        }

        private readonly ILogger _logger;
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly IMqttClientFactory _mqttFactory;
        private readonly ILifetimeScope _lifetimeScope;

        private const string _clientId = "Services.Wrapper.HomeAutomation";
        private readonly IList<TopicsContainer> _topicsContainers = new List<MqttManager.TopicsContainer>();
        private IMqttClient _mqttClient;

        public MqttManager(ILogger<MqttManager> logger,
            MqttConfiguration mqttConfiguration,
            IMqttClientFactory mqttFactory,
            ILifetimeScope lifetimeScope)
        {
            _logger = logger;
            _mqttConfiguration = mqttConfiguration;
            _mqttFactory = mqttFactory;
            _lifetimeScope = lifetimeScope;
        }

        public async Task ConnectAsync()
        {
            _logger.LogInformation("Connecting to Mosquitto");

            _mqttClient = _mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_mqttConfiguration.Hostname, _mqttConfiguration.Port)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(30))
                .WithCommunicationTimeout(TimeSpan.FromSeconds(30))
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

            _mqttClient.UseDisconnectedHandler(async e =>
            {
                _logger.LogWarning("Disconnected from MQTT server, reconnecting...");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _mqttClient.ConnectAsync(options);
                }
                catch
                {
                    _logger.LogWarning("Reconnecting to MQTT failed");
                }
            });
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
        {
            if (_mqttClient != null)
            {
                try
                {
                    var subscribedTopic = _lifetimeScope.Resolve<ISubscribedTopic<T>>();

                    await _mqttClient.SubscribeAsync(subscribedTopic.TopicName);

                    Func<string, Task> func = async payload =>
                    {
                        T param = default;

                        if (payload != null)
                        {
                            if (IsValidJson(payload))
                            {
                                param = JsonConvert.DeserializeObject<T>(payload);
                            }
                            else
                            {
                                if (typeof(T).IsAssignableTo<IMessageModel>())
                                {
                                    var instance = Activator.CreateInstance<T>() as IMessageModel;
                                    instance.Message = payload;
                                    param = (T)instance;
                                }
                                else
                                    throw new ArgumentException("Payload is simple string so model should implement IMessageModel");
                            }
                        }


                        _logger.LogInformation("Executing handler {handler}", subscribedTopic.GetType());
                        await subscribedTopic.Handle(param);
                    };

                    _topicsContainers.Add(new TopicsContainer(subscribedTopic.TopicName, func));

                    _mqttClient.UseApplicationMessageReceivedHandler(HandleReceivedMessages);
                    _logger.LogInformation("Subscribed {topicName} topic", subscribedTopic.TopicName);
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
            foreach (var topic in _topicsContainers.Where(t => t.TopicName == arg.ApplicationMessage.Topic))
            {
                var payload = arg.ApplicationMessage.Payload.Any() ?
                    System.Text.Encoding.Default.GetString(arg.ApplicationMessage.Payload) :
                    null;

                if (payload != null)
                    _logger.LogInformation("Execute function with topic {topic} and payload {payload}", topic.TopicName, payload);
                else
                    _logger.LogWarning("Execute function with topic {topic} AND NO PAYLOAD", topic.TopicName);

                await topic.FuncToExecute(payload);
            }
        }

        private bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
