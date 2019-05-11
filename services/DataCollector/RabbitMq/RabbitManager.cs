using Autofac;
using DataCollector.Config;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollector.RabbitMq
{
    public class RabbitManager
    {
        private IBusClient _busClient;

        private readonly RabbitMqConfig _rabbitMqConfiguration;
        private readonly ILogger _logger;

        public RabbitManager(RabbitMqConfig rabbitMqConfiguration,
            ILogger<RabbitManager> logger)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;
            _logger = logger;
        }

        public RabbitManager Connect()
        {
            bool isConnected = false;

            _logger.LogInformation("Connecting to RabbitMQ");

            do
            {
                try
                {
                    _busClient = BusClientFactory.CreateDefault(new RawRabbitConfiguration
                    {
                        Hostnames = { _rabbitMqConfiguration.Hostname },
                        Port = _rabbitMqConfiguration.Port,
                        AutomaticRecovery = true,
                        PersistentDeliveryMode = true,
                        AutoCloseConnection = true,
                        TopologyRecovery = true
                    });


                    isConnected = true;
                }
                catch (ConnectFailureException)
                {
                    _logger.LogWarning("Failed to connect to RabbitMQ, reconnecting...");
                    Task.Delay(2000).Wait();
                }
            }
            while (!isConnected);

            return this;
        }

        public async Task PublishAsync<T>(T model)
        {
            if (_busClient == null)
                Connect();

            await _busClient.PublishAsync(model);
        }
    }
}
