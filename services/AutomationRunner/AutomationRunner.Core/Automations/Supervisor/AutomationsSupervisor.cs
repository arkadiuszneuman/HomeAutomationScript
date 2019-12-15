﻿using AutomationRunner.Core.Automations.Specific;
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
        private readonly IEnumerable<IAutomation> automations;
        private readonly HomeAssistantConnector connector;

        public AutomationsSupervisor(
            ILogger<AutomationsSupervisor> logger,
            IEnumerable<IAutomation> automations,
            HomeAssistantConnector connector)
        {
            this.logger = logger;
            this.automations = automations;
            this.connector = connector;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            decimal previousValue = -1;
            logger.LogInformation($"Starting automations {string.Join(", ", automations.Select(a => a.GetType().Name))}");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await connector.RefreshStates();

                    var updateTasks = automations.Select(a => a.Update());
                    Task.WaitAll(updateTasks.ToArray(), cancellationToken);

                    //foreach (var automation in automations)
                    //{
                    //var entity = await automation.LoadEntity();
                    //var value = automation.Watch(entity);
                    //if (value != previousValue)
                    //{
                    //    await automation.Update();
                    //previousValue = value;
                    //    }
                    //}
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
    }
}