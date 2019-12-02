using AutomationRunner.Common.EntityLoader;
using AutomationRunner.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly EntityLoader entityLoader;

        public DaemonService(EntityLoader entityLoader)
        {
            this.entityLoader = entityLoader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 50; i++)
            {
                var entity = await XiaomiAirPurifier.LoadFromEntityId(entityLoader, "fan.air_purifier_pro");
            }

            Console.WriteLine("Asd");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
