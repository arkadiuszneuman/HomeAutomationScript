using AutomationRunner.Core.Automations.Specific;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Connector.Responses;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly IEnumerable<IStateAutomation> stateAutomations;

        public AutomationsSupervisor(
            ILogger<AutomationsSupervisor> logger,
            IEnumerable<IAutomation> automations,
            HomeAssistantConnector connector,
            HomeAssistantWebSocketConnector webSocketConnector,
            IEnumerable<IStateAutomation> stateAutomations)
        {
            this.logger = logger;
            this.automations = automations;
            this.connector = connector;
            this.webSocketConnector = webSocketConnector;
            this.stateAutomations = stateAutomations;
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
                catch (OperationCanceledException)
                {
                    // ignore
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
            
            foreach (var automation in stateAutomations.Where(s => s.EntityName == oldNewState.NewState.EntityId))
                await automation.Update(oldNewState.OldState, oldNewState.NewState);
        }
    }
}
