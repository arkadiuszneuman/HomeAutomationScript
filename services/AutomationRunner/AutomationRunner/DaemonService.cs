using AutomationRunner.Common.HomeAssistantConnector;
using AutomationRunner.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly HomeAssistantConnector entityLoader;

        public DaemonService(HomeAssistantConnector entityLoader)
        {
            this.entityLoader = entityLoader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var entity = await XiaomiAirPurifier.LoadFromEntityId(entityLoader, "fan.air_purifier_2s");

            var currentState = false;

            Console.WriteLine("Asd");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
