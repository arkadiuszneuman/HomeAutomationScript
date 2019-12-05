using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common.Connector;
using AutomationRunner.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Fan.BedroomAirPurifier
{
    public class TurnOffIfAqiIsBelow20
    {
        private const double forTime = 0.5;
        private const int turningOffValue = 20;

        private readonly ILogger<TurnOffIfAqiIsBelow20> logger;
        private readonly HomeAssistantConnector connector;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public Task<XiaomiAirPurifier> Entity => XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_pro");
        public Func<XiaomiAirPurifier, decimal> Watch => entity => entity.Attributes.Aqi;

        public TurnOffIfAqiIsBelow20(
            ILogger<TurnOffIfAqiIsBelow20> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory)
        {
            this.logger = logger;
            this.connector = connector;
            this.turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTime));
        }

        public async Task Update()
        {
            var airPurifier = await Entity;

            if (turnOffCondition.CheckFulfilled(airPurifier.Attributes.Aqi <= turningOffValue))
            {
                logger.LogInformation("Turning off {0}, because aqi is lower or equal than {1} for {2} minutes",
                    airPurifier.EntityId, turningOffValue, forTime);
                await airPurifier.TurnOff();
            }

            if (turnOnCondition.CheckFulfilled(airPurifier.Attributes.Aqi > turningOffValue))
            {
                logger.LogInformation("Turning on {0}, because aqi is bigger than {1} for {2} minutes",
                    airPurifier.EntityId, turningOffValue, forTime);
                await airPurifier.TurnOn();
            }
        }
    }
}
