using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirHumidifier
{
    public class AirHumidifierAutomations : IAutomation
    {
        private const double forTime = 3;
        private const int turningOnValue = 60;

        private readonly ILogger<AirHumidifierAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public Task<XiaomiAirPurifier> LoadAirPurifierProEntity() => XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_pro");
        public Task<XiaomiAirHumidifier> LoadAirHumidifierEntity() => XiaomiAirHumidifier.LoadFromEntityId(connector, "fan.air_humidifier");

        public AirHumidifierAutomations(
            ILogger<AirHumidifierAutomations> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
            this.turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .Name(logger, "Air humidifier turning off")
                .For(TimeSpan.FromMinutes(forTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .Name(logger, "Air humidifier turning on")
                .For(TimeSpan.FromMinutes(forTime));
        }

        public async Task Update()
        {
            var airPurifierPro = await LoadAirPurifierProEntity();
            var airHumidifer = await LoadAirHumidifierEntity();

            logger.LogDebug("Checking air humidifier. Humidity: {0}", airPurifierPro.Attributes.Humidity);
            if (dateTimeHelper.Now.Between(new TimeSpan(0, 0, 0), new TimeSpan(6, 0, 0)))
            {
                logger.LogDebug("Air humidifier should be turned off because of night");
                if (airHumidifer.State == "on")
                {
                    logger.LogInformation("Turning off {0}, because of night", airHumidifer.EntityId);
                    await airHumidifer.TurnOff();
                }

                return;
            }

            if (turnOffCondition.CheckFulfilled(airPurifierPro.Attributes.Humidity > turningOnValue))
            {
                if (airHumidifer.State == "on")
                {
                    logger.LogInformation("Turning off {0}, because aqi is bigger than {1} for {2} minutes",
                    airHumidifer.EntityId, turningOnValue, forTime);
                    await airHumidifer.TurnOff();
                }
            }

            if (turnOnCondition.CheckFulfilled(airPurifierPro.Attributes.Humidity <= turningOnValue))
            {
                if (airHumidifer.State == "off")
                {
                    logger.LogInformation("Turning on {0}, because aqi is lower or equal than {1} for {2} minutes",
                    airHumidifer.EntityId, turningOnValue, forTime);
                    await airHumidifer.TurnOn();
                }
            }

            if (airHumidifer.State == "on")
            {
                var changeHumidityInfo = airPurifierPro.Attributes.Humidity switch
                {
                    var humidity when humidity <= 40 => new { Change = true, Speed = AirHumidifierSpeed.High },
                    var humidity when humidity <= 50 => new { Change = true, Speed = AirHumidifierSpeed.Medium },
                    var humidity when humidity <= turningOnValue => new { Change = true, Speed = AirHumidifierSpeed.Silent },
                    _ => new { Change = false, Speed = AirHumidifierSpeed.Auto }
                };

                logger.LogDebug("Change humidity info. Change:{0}, Speed: {1}", changeHumidityInfo.Change, changeHumidityInfo.Speed);

                if (changeHumidityInfo.Change && changeHumidityInfo.Speed != airHumidifer.Attributes.Speed)
                {
                    logger.LogInformation("Changing speed of {0} from {1} to {2}",
                        airHumidifer.EntityId, airHumidifer.Attributes.Speed, changeHumidityInfo.Speed);
                    await airHumidifer.SetSpeed(changeHumidityInfo.Speed);
                }
            }
        }
    }
}
