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
    public class AirPurifierProAutomations : IAutomation
    {
        private const double forTime = 3;
        private const int turningOffValue = 15;

        private readonly ILogger<AirPurifierProAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public Task<XiaomiAirPurifier> LoadEntity() => XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_pro");
        public Func<XiaomiAirPurifier, decimal> Watch => entity => entity.Aqi;

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

        public async Task Update()
        {
            var airPurifier = await LoadEntity();

            if (dateTimeHelper.Now.Between(new TimeSpan(23, 0, 0), new TimeSpan(6, 0, 0)))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because of night", airPurifier.EntityId);
                    await airPurifier.TurnOff();
                }

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
                    if (airPurifier.Speed != AirPurifierSpeed.Auto)
                    {
                        logger.LogInformation("Changing speed of {EntityId} to {Speed}",
                            airPurifier.EntityId, AirPurifierSpeed.Auto);
                        await airPurifier.SetSpeed(AirPurifierSpeed.Auto);
                    }
                }
                else
                {
                    var level = Math.Min((airPurifier.Aqi / 10) + 1, 16);

                    if (airPurifier.Speed != AirPurifierSpeed.Favorite ||
                        airPurifier.FavoriteLevel != level)
                    {
                        logger.LogInformation("Changing speed of {EntityId} to {Level}",
                               airPurifier.EntityId, level);

                        await airPurifier.SetLevel(level);
                        await airPurifier.SetSpeed(AirPurifierSpeed.Favorite);
                    }
                }
            }
        }
    }
}
