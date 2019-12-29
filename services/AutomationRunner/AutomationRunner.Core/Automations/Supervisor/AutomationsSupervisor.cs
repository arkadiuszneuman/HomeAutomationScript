using AutomationRunner.Core.Automations.Specific;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly HomeAssistantConnector connector;
        private readonly HomeAssistantWebSocketConnector webSocketConnector;
        private readonly IEnumerable<IEntityStateAutomation> stateAutomations;
        private readonly IEnumerable<IEntitiesStateAutomation> entitesStatesAutomations;
        private readonly IEnumerable<ITimeUpdate> timeUpdateAutomations;
        private readonly IDateTimeHelper dateTimeHelper;

        private readonly Dictionary<ITimeUpdate, DateTime> executedTimeUpdateAutomations
            = new Dictionary<ITimeUpdate, DateTime>();

        public AutomationsSupervisor(
            ILogger<AutomationsSupervisor> logger,
            HomeAssistantConnector connector,
            HomeAssistantWebSocketConnector webSocketConnector,
            IEnumerable<IEntityStateAutomation> entityStateAutomations,
            IEnumerable<IEntitiesStateAutomation> entitesStatesAutomations,
            IEnumerable<ITimeUpdate> timeUpdateAutomations,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.webSocketConnector = webSocketConnector;
            this.stateAutomations = entityStateAutomations;
            this.entitesStatesAutomations = entitesStatesAutomations;
            this.timeUpdateAutomations = timeUpdateAutomations;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting time update automations {Automations}", timeUpdateAutomations.Select(a => a.GetType().Name));
            webSocketConnector.SubscribeStateChanged(OnStateChanged);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var now = dateTimeHelper.Now;

                    if (executedTimeUpdateAutomations.Any(keyValue => now - keyValue.Key.UpdateEvery > keyValue.Value))
                        await connector.RefreshStates();

                    foreach (var timeUpdateAutomation in timeUpdateAutomations)
                    {
                        if (!executedTimeUpdateAutomations.ContainsKey(timeUpdateAutomation))
                        {
                            logger.LogInformation("Updating {Type} - at start", timeUpdateAutomation.GetType().Name);
                            timeUpdateAutomation.Update();
                            executedTimeUpdateAutomations.Add(timeUpdateAutomation, now);
                        }
                        else
                        {
                            var lastUpdate = executedTimeUpdateAutomations[timeUpdateAutomation];
                            if (now - timeUpdateAutomation.UpdateEvery >= lastUpdate)
                            {
                                logger.LogInformation("Updating {Type} - time to update have passed", timeUpdateAutomation.GetType().Name);
                                timeUpdateAutomation.Update();
                                executedTimeUpdateAutomations[timeUpdateAutomation] = now;
                            }
                        }
                    }
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

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
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
