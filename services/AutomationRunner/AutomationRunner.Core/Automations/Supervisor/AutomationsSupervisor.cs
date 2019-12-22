using AutomationRunner.Core.Automations.Specific;
using AutomationRunner.Core.Common.Connector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Supervisor
{
    public class AutomationsSupervisor
    {
        private readonly ILogger<AutomationsSupervisor> logger;
        private readonly IEnumerable<IAutomation> automations;
        private readonly HomeAssistantConnector connector;
        private readonly HomeAssistantWebSocketConnector webSocketConnector;
        private readonly IEnumerable<IEntityStateAutomation> stateAutomations;
        private readonly IEnumerable<IEntitiesStateAutomation> entitesStatesAutomations;

        public AutomationsSupervisor(
            ILogger<AutomationsSupervisor> logger,
            IEnumerable<IAutomation> automations,
            HomeAssistantConnector connector,
            HomeAssistantWebSocketConnector webSocketConnector,
            IEnumerable<IEntityStateAutomation> entityStateAutomations,
            IEnumerable<IEntitiesStateAutomation> entitesStatesAutomations)
        {
            this.logger = logger;
            this.automations = automations;
            this.connector = connector;
            this.webSocketConnector = webSocketConnector;
            this.stateAutomations = entityStateAutomations;
            this.entitesStatesAutomations = entitesStatesAutomations;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting automations {Automations}", automations.Select(a => a.GetType().Name));

            webSocketConnector.SubscribeStateChanged(OnStateChanged);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await connector.RefreshStates();

                    foreach (var automation in automations)
                        await automation.Update();
                }
                catch (HttpRequestException)
                {
                    logger.LogWarning($"Cannot connect HomeAssistant. Retrying...");
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Error on executing automations. Waiting minute and restarting...");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }

                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task OnStateChanged(OldNewState oldNewState)
        {
            await connector.RefreshStates();

            IEnumerable<IStateUpdate> singleEntityStateAutomations = stateAutomations
                .Where(s => s.EntityName == oldNewState.NewState.EntityId);

            IEnumerable<IStateUpdate> manyEntitiesStateEutomations = entitesStatesAutomations
                .Where(s => s.EntityNames.Contains(oldNewState.NewState.EntityId));

            foreach (var automation in singleEntityStateAutomations.Union(manyEntitiesStateEutomations))
            {
                logger.LogInformation("Starting state automation {automation}", automation);
                automation.Update(oldNewState.OldState, oldNewState.NewState);
            }
        }
    }
}
