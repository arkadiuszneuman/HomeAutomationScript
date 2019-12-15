using AutomationRunner.Core.Automations.Supervisor;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly AutomationsSupervisor automationsSupervisor;

        public DaemonService(AutomationsSupervisor automationsSupervisor)
        {
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
