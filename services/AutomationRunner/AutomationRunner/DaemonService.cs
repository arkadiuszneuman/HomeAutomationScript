using AutomationRunner.Core.Automations.Supervisor;
using AutomationRunner.Core.Scenes;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly AutomationsSupervisor automationsSupervisor;
        private readonly ScenesActivator scenesSupervisor;

        public DaemonService(AutomationsSupervisor automationsSupervisor,
            ScenesActivator scenesSupervisor)
        {
            this.automationsSupervisor = automationsSupervisor;
            this.scenesSupervisor = scenesSupervisor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.WhenAll(
                    automationsSupervisor.Start(cancellationToken),
                    scenesSupervisor.Start(cancellationToken));
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
