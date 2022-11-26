using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan
{
    public class AirPurifierProAutomations : IEntityStateAutomation, ITimeUpdate
    {
        private const double forTime = 5;
        private const int turningOffValue = 15;

        private readonly ILogger<AirPurifierProAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public string EntityName => XiaomiAirPurifier.Name.AirPurifierPro.GetEntityId();
        
        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(forTime);

        public AirPurifierProAutomations(
            ILogger<AirPurifierProAutomations> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
            this.turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTime));
        }

        public Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            return Update();
        }

        public async Task Update()
        {
            var airPurifier = await XiaomiAirPurifier.LoadFromEntityId(connector, XiaomiAirPurifier.Name.AirPurifierPro);

            if (dateTimeHelper.Now.Between(new TimeSpan(23, 0, 0), new TimeSpan(6, 0, 0)))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because of night", airPurifier.EntityId);
                    await airPurifier.TurnOff();
                }
            
                turnOnCondition.Reset();
                turnOffCondition.Reset();
            
                return;
            }
            
            if (turnOffCondition.CheckFulfilled(airPurifier.Aqi <= turningOffValue))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because aqi is lower or equal than {TurningOffValue} for {ForTime} minutes",
                        airPurifier.EntityId, turningOffValue, forTime);
                    await airPurifier.TurnOff();
                }
            }
            
            if (turnOnCondition.CheckFulfilled(airPurifier.Aqi > turningOffValue))
            {
                if (airPurifier.State == "off")
                {
                    logger.LogInformation("Turning on {EntityId}, because aqi is lower or equal than {TurningOffValue} for {ForTime} minutes",
                        airPurifier.EntityId, turningOffValue, forTime);
                    await airPurifier.TurnOn();
                }
            }
            
            if (airPurifier.State == "on")
            {
                if (airPurifier.Aqi < 20)
                {
                    if (airPurifier.PresetMode != AirPurifierPresetMode.Auto)
                    {
                        logger.LogInformation("Changing speed of {EntityId} to {Speed}",
                            airPurifier.EntityId, AirPurifierPresetMode.Auto);
                        await airPurifier.SetPresetMode(AirPurifierPresetMode.Auto);
                    }
                }
                else
                {
                    var level = Math.Min((airPurifier.Aqi / 10) + 1, 16);
            
                    if (airPurifier.PresetMode != AirPurifierPresetMode.Favorite ||
                        airPurifier.FavoriteLevel != level)
                    {
                        logger.LogInformation("Changing speed of {EntityId} to {Level}",
                               airPurifier.EntityId, level);
            
                        await airPurifier.SetFavoriteLevel(level);
                        await airPurifier.SetPresetMode(AirPurifierPresetMode.Favorite);
                    }
                }
            }
        }
    }
}
