using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.Config;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant
{
    public class DaemonService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly DaemonConfiguration _config;
        private readonly RabbitMqConfiguration _rabbitConfiguration;
        private readonly MqttConfiguration _mqttConfiguration;

        private DateTime LastTimeBottomSensorDetected = DateTime.MinValue;
        private DateTime LastTimeUpperSensorDetected = DateTime.MinValue;

        public DaemonService(ILogger<DaemonService> logger, DaemonConfiguration config,
            RabbitMqConfiguration rabbitConfiguration,
            MqttConfiguration mqttConfiguration)
        {
            _logger = logger;
            _config = config;
            _rabbitConfiguration = rabbitConfiguration;
            _mqttConfiguration = mqttConfiguration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.DaemonName);

            Console.WriteLine("Hello World!");

            var busClient = BusClientFactory.CreateDefault(new RawRabbitConfiguration
            {
                Hostnames = { _rabbitConfiguration.Hostname },
                Port = _rabbitConfiguration.Port
            });
           

            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Services.Wrapper.HomeAutomation")
                .WithTcpServer(_mqttConfiguration.Hostname, _mqttConfiguration.Port)
                .Build();

            client.Connected += async (s, e) =>
            {
                Console.WriteLine("MQTT connected");

                //await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test").Build());
            };


            var x = client.ConnectAsync(options).Result;

            busClient.SubscribeAsync<TriggeredUpperStairSensorModel>(async (msg, context) =>
            {
                Console.WriteLine($"Triggered upper stairs sensor at {msg.DateTime}");

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("home/stairs/upper_motion_detector/set")
                    .WithPayload("ON")
                    .Build();

                await client.PublishAsync(message);

                var differenceBetweenSensorsTime = DateTime.Now - LastTimeBottomSensorDetected;

                if (differenceBetweenSensorsTime >= TimeSpan.FromSeconds(3) &&
                    differenceBetweenSensorsTime <= TimeSpan.FromSeconds(30))
                {
                    LastTimeBottomSensorDetected = DateTime.MinValue;

                    Console.WriteLine($"Sending set_going_up on MQTT");

                    message = new MqttApplicationMessageBuilder()
                        .WithTopic("home/stairs/set_going_up")
                        .WithPayload("ON")
                        .Build();

                    await client.PublishAsync(message);
                }

                LastTimeUpperSensorDetected = msg.DateTime;

                await Task.Delay(5000);

                message = new MqttApplicationMessageBuilder()
                    .WithTopic("home/stairs/upper_motion_detector/set")
                    .WithPayload("OFF")
                    .Build();

                await client.PublishAsync(message);

                message = new MqttApplicationMessageBuilder()
                   .WithTopic("home/stairs/set_going_up")
                   .WithPayload("OFF")
                   .Build();

                await client.PublishAsync(message);
            });

            busClient.SubscribeAsync<TriggeredBottomStairSensorModel>(async (msg, context) =>
            {
                Console.WriteLine($"Triggered bottom stairs sensor at {msg.DateTime}");

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("home/stairs/bottom_motion_detector/set")
                    .WithPayload("ON")
                    .Build();

                await client.PublishAsync(message);

                var differenceBetweenSensorsTime = DateTime.Now - LastTimeUpperSensorDetected;
                if (differenceBetweenSensorsTime >= TimeSpan.FromSeconds(3) &&
                    differenceBetweenSensorsTime <= TimeSpan.FromSeconds(30))
                {
                    LastTimeUpperSensorDetected = DateTime.MinValue;

                    Console.WriteLine($"Sending set_going_down on MQTT");

                    message = new MqttApplicationMessageBuilder()
                        .WithTopic("home/stairs/set_going_down")
                        .WithPayload("ON")
                        .Build();

                    await client.PublishAsync(message);
                }


                LastTimeBottomSensorDetected = msg.DateTime;

                await Task.Delay(5000);

                message = new MqttApplicationMessageBuilder()
                    .WithTopic("home/stairs/bottom_motion_detector/set")
                    .WithPayload("OFF")
                    .Build();

                await client.PublishAsync(message);

                message = new MqttApplicationMessageBuilder()
                   .WithTopic("home/stairs/set_going_down")
                   .WithPayload("OFF")
                   .Build();

                await client.PublishAsync(message);
            });

            client.Disconnected += async (s, e) =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await client.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping daemon.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");

        }
    }
}
