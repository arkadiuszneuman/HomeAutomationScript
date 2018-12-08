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
        private readonly IOptions<DaemonConfig> _config;
        private readonly RawRabbitConfiguration _rabbitConfiguration;
        private readonly MqttConfiguration _mqttConfiguration;

        public DaemonService(ILogger<DaemonService> logger, IOptions<DaemonConfig> config,
            IOptions<RawRabbitConfiguration> rabbitConfiguration,
            MqttConfiguration mqttConfiguration)
        {
            _logger = logger;
            _config = config;
            _rabbitConfiguration = rabbitConfiguration.Value;
            _mqttConfiguration = mqttConfiguration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.Value.DaemonName);


            Console.WriteLine("Hello World!");

            Console.WriteLine($"RABBITMQ HOSTNAMES: {string.Join(',', _rabbitConfiguration.Hostnames)}");

            var busClient = BusClientFactory.CreateDefault(_rabbitConfiguration);
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
