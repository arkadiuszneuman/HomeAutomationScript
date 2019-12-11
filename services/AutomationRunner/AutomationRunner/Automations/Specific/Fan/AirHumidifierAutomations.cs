using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common;
using AutomationRunner.Common.Connector;
using AutomationRunner.Common.Extensions;
using AutomationRunner.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Specific.Fan
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
                .For(TimeSpan.FromMinutes(forTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTime));
        }

        public async Task Update()
        {
            var airPurifierPro = await LoadAirPurifierProEntity();
            var airHumidifer = await LoadAirHumidifierEntity();

            if (dateTimeHelper.Now.Between(new TimeSpan(0, 0, 0), new TimeSpan(6, 0, 0)))
            {
                if (airHumidifer.State == "on")
                {
                    logger.LogInformation("Turning off {0}, because of night", airHumidifer.EntityId);
                    await airHumidifer.TurnOff();
                }

                return;
            }

            if (turnOffCondition.CheckFulfilled(airPurifierPro.Attributes.Humidity > turningOnValue))
            {
                logger.LogInformation("Turning off {0}, because aqi is bigger than {1} for {2} minutes",
                    airHumidifer.EntityId, turningOnValue, forTime);
                await airHumidifer.TurnOff();
            }

            if (turnOnCondition.CheckFulfilled(airPurifierPro.Attributes.Humidity <= turningOnValue))
            {
                logger.LogInformation("Turning on {0}, because aqi is lower or equal than {1} for {2} minutes",
                    airHumidifer.EntityId, turningOnValue, forTime);
                await airHumidifer.TurnOn();
            }

            if (airHumidifer.State == "on")
            {
                var speed = airPurifierPro.Attributes.Humidity switch
                {
                    var humidity when humidity <= 40 => AirHumidifierSpeed.High,
                    var humidity when humidity <= 50 => AirHumidifierSpeed.Medium,
                    var humidity when humidity <= turningOnValue => AirHumidifierSpeed.Silent
                };

                if (speed != airHumidifer.Attributes.Speed)
                {
                    logger.LogInformation("Changing speed of {0} from {1} to {2}", 
                        airHumidifer.EntityId, airHumidifer.Attributes.Speed, speed);
                    await airHumidifer.SetSpeed(speed);
                }
            }
        }
    }
}
