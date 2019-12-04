using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common.HomeAssistantConnector;
using AutomationRunner.Entities;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Fan.BedroomAirPurifier
{
    public class TurnOffIfAqiIsBelow20
    {
        private readonly HomeAssistantConnector connector;
        private readonly ConditionHelper conditionsHelper;

        //public override Task<XiaomiAirPurifier> Entity => XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_2s");

        public TurnOffIfAqiIsBelow20(HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory)
        {
            this.connector = connector;
            this.conditionsHelper = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(3));
        }

        public async Task Update()
        {
            var airPurifier = await XiaomiAirPurifier.LoadFromEntityId(connector, "fan.air_purifier_2s");

            if (conditionsHelper.CheckFulfilled(airPurifier.Attributes.Aqi < 20))
                await airPurifier.TurnOff();
        }
    }
}
