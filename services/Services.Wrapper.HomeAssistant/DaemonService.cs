using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Hostnames = new List<string>() { _rabbitConfiguration.Hostname },
                Port = _rabbitConfiguration.Port
            });
            busClient.SubscribeAsync<TestModel>((async (msg, context) =>
            {
                Console.WriteLine(msg.Message);
            }));

            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Services.Wrapper.HomeAutomation")
                .WithTcpServer(_mqttConfiguration.Hostname, _mqttConfiguration.Port)
                .Build();

            client.Connected += async (s, e) =>
            {
                Console.WriteLine("MQTT connected");
            };

            var x = client.ConnectAsync(options).Result;

            Console.WriteLine("Arek");



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
