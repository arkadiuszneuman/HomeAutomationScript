using AutomationRunner.Automations.Supervisor;
using AutomationRunner.Common.Connector;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly HomeAssistantConnector entityLoader;
        private readonly AutomationsSupervisor automationsSupervisor;

        public DaemonService(
            HomeAssistantConnector entityLoader,
            AutomationsSupervisor automationsSupervisor)
        {
            this.entityLoader = entityLoader;
            this.automationsSupervisor = automationsSupervisor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await automationsSupervisor.Start(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
