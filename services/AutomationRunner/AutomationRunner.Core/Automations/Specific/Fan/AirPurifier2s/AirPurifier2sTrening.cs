using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirPurifier2s
{
    public class AirPurifier2sTrening : IEntityStateAutomation, ITimeUpdate, IAirPurifiers2sAutomationType
    {
        private const double forTurnOnTime = 3;
        private const double forTurnOffTime = 3;
        private const int turningOffValue = 5;

        private readonly ILogger<AirPurifier2sTrening> logger;
        private readonly HomeAssistantConnector connector;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public string EntityName { get; } = XiaomiAirPurifier.Name.AirPurifier2S.GetEntityId();
        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(Math.Min(forTurnOnTime, forTurnOffTime));
        public string AutomationType { get; } = "Trening na bieżni";

        public AirPurifier2sTrening(
            ILogger<AirPurifier2sTrening> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory)
        {
            this.logger = logger;
            this.connector = connector;
            this.turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOffTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOnTime));
        }

        public Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            return Update();
        }

        public async Task Update()
        {
            var automationType = await InputSelect.LoadFromEntityId(connector, InputSelect.Name.AirPurifier2sAutomationType);
            if (automationType.State != AutomationType)
            {
                ClearConditions();
                return;
            }

            var airPurifier = await XiaomiAirPurifier.LoadFromEntityId(connector, XiaomiAirPurifier.Name.AirPurifier2S);

            if (turnOffCondition.CheckFulfilled(airPurifier.Aqi <= turningOffValue))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because aqi is lower or equal than {TurningOffValue} for {ForTime} minutes",
                        airPurifier.EntityId, turningOffValue, forTurnOnTime);
                    await airPurifier.TurnOff();
                }
            }

            if (turnOnCondition.CheckFulfilled(airPurifier.Aqi > turningOffValue))
            {
                if (airPurifier.State == "off")
                {
                    logger.LogInformation("Turning on {EntityId}, because aqi is lower or equal than {TurningOffValue} for {ForTime} minutes",
                        airPurifier.EntityId, turningOffValue, forTurnOnTime);
                    await airPurifier.TurnOn();
                }
            }

            if (airPurifier.State == "on")
            {
                var level = Math.Min((airPurifier.Aqi / 10) + 3, 16);

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

        private void ClearConditions()
        {
            turnOnCondition.Reset();
            turnOffCondition.Reset();
        }
    }
}
