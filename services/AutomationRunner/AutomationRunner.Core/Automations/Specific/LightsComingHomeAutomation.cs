using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific
{
    public class LightsComingHomeAutomation : IEntitiesStateAutomation
    {
        private readonly ILogger<LightsComingHomeAutomation> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;

        public IEnumerable<string> EntityNames => new[] { 
            DeviceTracker.Name.Arek.GetEntityId(),
            DeviceTracker.Name.Patrycja.GetEntityId()
        };

        public LightsComingHomeAutomation(ILogger<LightsComingHomeAutomation> logger,
            HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            var oldState = InputNumber.CreateBasedOnBaseEntity(oldStateBaseEntity);
            var newState = InputNumber.CreateBasedOnBaseEntity(newStateBaseEntity);

            if (oldState.State == "not_home" && newState.State == "home")
            {
                logger.LogDebug("{User} came home", newState.EntityId);

                var sun = await Sun.Load(connector, dateTimeHelper);

                logger.LogDebug("Sun: {SunNextSettingUtc}, CurrentDate {DateUtc}", sun.NextSettingUtc, dateTimeHelper.UtcNow);

                if (sun.AfterSunset(TimeSpan.FromMinutes(-30)))
                {
                    logger.LogInformation("There is after sunset minus 30 mins. Executing external lights");

                    var externalLight = await Light.LoadFromEntityId(connector, Light.Name.ExternalLight);
                    var gardenLight = await Switch.LoadFromEntityId(connector, Switch.Name.GardenLight);

                    await externalLight.TurnOn();
                    await gardenLight.TurnOn();

                    await Task.Delay(TimeSpan.FromMinutes(10));

                    await externalLight.TurnOff();
                    await gardenLight.TurnOff();
                }
            }
        }
    }
}
