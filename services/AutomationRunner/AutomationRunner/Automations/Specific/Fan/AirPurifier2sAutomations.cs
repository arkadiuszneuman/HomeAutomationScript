using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common;
using AutomationRunner.Common.Connector;
using AutomationRunner.Common.Extensions;
using AutomationRunner.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Specific.Fan
{
    public class AirPurifier2sAutomations : IAutomation
    {
        private const double forTime = 3;
        private const int turningOffValue = 10;

        private readonly ILogger<AirPurifier2sAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public Task<XiaomiAirPurifier> LoadEntity() => XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_2s");
        public Func<XiaomiAirPurifier, decimal> Watch => entity => entity.Attributes.Aqi;

        public AirPurifier2sAutomations(
            ILogger<AirPurifier2sAutomations> logger,
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

            if (dateTimeHelper.Now.Between(new TimeSpan(8, 0, 0), new TimeSpan(19, 0, 0)))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {0}, because of no one in the room", airPurifier.EntityId);
                    await airPurifier.TurnOff();
                }
            }
            else
            {
                if (turnOffCondition.CheckFulfilled(airPurifier.Attributes.Aqi <= turningOffValue))
                {
                    if (airPurifier.State == "on")
                    {
                        logger.LogInformation("Turning off {0}, because aqi is lower or equal than {1} for {2} minutes",
                            airPurifier.EntityId, turningOffValue, forTime);
                        await airPurifier.TurnOff();
                    }
                }

                if (turnOnCondition.CheckFulfilled(airPurifier.Attributes.Aqi > turningOffValue))
                {
                    if (airPurifier.State == "off")
                    {
                        logger.LogInformation("Turning on {0}, because aqi is bigger than {1} for {2} minutes",
                            airPurifier.EntityId, turningOffValue, forTime);
                        await airPurifier.TurnOn();
                    }
                }

                if (airPurifier.State == "on")
                {
                    if (dateTimeHelper.Now.Between(new TimeSpan(19, 0, 0), new TimeSpan(21, 0, 0)))
                    {
                        var level = Math.Min((airPurifier.Attributes.Aqi / 10) + 2, 16);

                        if (airPurifier.Attributes.Speed != AirPurifierSpeed.Favorite ||
                            airPurifier.Attributes.FavoriteLevel != level)
                        {
                            logger.LogInformation("Changing speed of {0} to {1}",
                                    airPurifier.EntityId, level);

                            await airPurifier.SetLevel(level);
                            await airPurifier.SetSpeed(AirPurifierSpeed.Favorite);
                        }
                    }
                    else
                    {
                        if (airPurifier.Attributes.Aqi <= 20)
                        {
                            if (airPurifier.Attributes.Speed != AirPurifierSpeed.Silent)
                            {
                                logger.LogInformation("Changing speed of {0} to {1}",
                                       airPurifier.EntityId, AirPurifierSpeed.Silent);

                                await airPurifier.SetSpeed(AirPurifierSpeed.Silent);
                            }
                            
                        }
                        else
                        {
                            if (airPurifier.Attributes.Speed != AirPurifierSpeed.Auto)
                            {
                                logger.LogInformation("Changing speed of {0} to Auto",
                                    airPurifier.EntityId);
                                await airPurifier.SetSpeed(AirPurifierSpeed.Auto);
                            }
                        }
                    }
                }
            }
        }
    }
}
