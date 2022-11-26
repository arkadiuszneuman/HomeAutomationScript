using AutomationRunner.Core.Common.Exceptions;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirPurifier2s
{
    public class ChangedAirPurifier2sAutomationType : IEntityStateAutomation
    {
        private readonly ILogger<ChangedAirPurifier2sAutomationType> logger;
        private readonly IEnumerable<IAirPurifiers2sAutomationType> automationTypes;

        public string EntityName { get; } = InputSelect.Name.AirPurifier2sAutomationType.GetEntityId();

        public ChangedAirPurifier2sAutomationType(
            ILogger<ChangedAirPurifier2sAutomationType> logger,
            IEnumerable<IAirPurifiers2sAutomationType> automationTypes)
        {
            this.logger = logger;
            this.automationTypes = automationTypes;
        }

        public async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            if (oldStateBaseEntity.State == newStateBaseEntity.State)
                return;

            var automation = GetAutomation(newStateBaseEntity);
            logger.LogInformation("Changed input select option to {option}. Updating automation {automation}", newStateBaseEntity.State, automation.GetType().Name);
            await automation.Update();
        }

        private IAirPurifiers2sAutomationType GetAutomation(BaseEntity newStateBaseEntity)
        {
            var selectedAutomations = automationTypes.Where(a => a.AutomationType == newStateBaseEntity.State).ToList();
            if (selectedAutomations.Count > 1)
                throw new InvalidHomeAssistantConfigurationException($"Too many automations for input select option: {newStateBaseEntity.State}");
            if (selectedAutomations.Count == 0)
                throw new InvalidHomeAssistantConfigurationException($"No automations for input select option: {newStateBaseEntity.State}");

            return selectedAutomations.Single();
        }
    }
}
