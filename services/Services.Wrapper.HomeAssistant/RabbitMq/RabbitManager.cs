using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.RabbitMq
{
    public class RabbitManager
    {
        private IBusClient _busClient;

        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public RabbitManager(RabbitMqConfiguration rabbitMqConfiguration,
            ILifetimeScope lifetimeScope,
            ILogger<RabbitManager> logger)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;
            _lifetimeScope = lifetimeScope;
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

        public RabbitManager AddHandler<T>()
        {
            if (_busClient != null)
            {
                var handler = _lifetimeScope.Resolve<IHandler<T>>();
                _busClient.SubscribeAsync<T>(async (msg, context) => await handler.Execute(msg));
            }

            return this;
        }
    }
}
