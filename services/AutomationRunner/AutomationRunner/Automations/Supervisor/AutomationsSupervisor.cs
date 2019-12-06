using AutomationRunner.Automations.Fan.BedroomAirPurifier;
using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common.Connector;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Supervisor
{
    public class AutomationsSupervisor
    {
        private readonly ILogger<AutomationsSupervisor> logger;
        private readonly AirPurifierProAutomations automation;
        private readonly HomeAssistantConnector connector;

        public AutomationsSupervisor(
            ILogger<AutomationsSupervisor> logger,
            AirPurifierProAutomations automation,
            HomeAssistantConnector connector)
        {
            this.logger = logger;
            this.automation = automation;
            this.connector = connector;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            decimal previousValue = -1;

            while (!cancellationToken.IsCancellationRequested)
            {
                await connector.RefreshStates();

                var entity = await automation.LoadEntity();
                var value = automation.Watch(entity);
                //if (value != previousValue)
                {
                    await automation.Update();
                    previousValue = value;
                }

                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}
