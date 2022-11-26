using System;
using System.Threading.Tasks;
using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirPurifier2s
{
    public class AirPurifier2sSleepingRoom : IEntityStateAutomation, ITimeUpdate, IAirPurifiers2sAutomationType
    {
        private const double forTurnOnTime = 5;
        private const double forTurnOffTime = 15;
        private const double speedChangeForTime = 20;
        private const int turningOffValue = 10;

        private readonly ILogger<AirPurifier2sSleepingRoom> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;
        private readonly ConditionHelper silentSpeedCondition;
        private readonly ConditionHelper autoSpeedCondition;

        public string EntityName { get; } = XiaomiAirPurifier.Name.AirPurifier2S.GetEntityId();

        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(Math.Min(Math.Min(forTurnOnTime, forTurnOffTime), speedChangeForTime)) +
                                               TimeSpan.FromSeconds(30);

        public string AutomationType { get; } = "Sypialnia";

        public AirPurifier2sSleepingRoom(
            ILogger<AirPurifier2sSleepingRoom> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
            turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOffTime));

            turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOnTime));

            silentSpeedCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(speedChangeForTime));

            autoSpeedCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(speedChangeForTime));
        }

        public Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
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

            if (dateTimeHelper.Now.Between(new TimeSpan(9, 30, 0), new TimeSpan(12, 0, 0)))
            {
                if (airPurifier.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because of no one in the room", airPurifier.EntityId);
                    await airPurifier.TurnOff();
                }

                ClearConditions();
            }
            else
            {
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
                        logger.LogInformation("Turning on {EntityId}, because aqi is greater or equal than {TurningOffValue} for {ForTime} minutes",
                            airPurifier.EntityId, turningOffValue, forTurnOnTime);
                        await airPurifier.TurnOn();

                        if (airPurifier.PresetMode != AirPurifierPresetMode.Silent)
                        {
                            logger.LogInformation("Changing speed of {EntityId} to {Speed} after turning on",
                                airPurifier.EntityId, AirPurifierPresetMode.Silent);

                            await airPurifier.SetPresetMode(AirPurifierPresetMode.Silent);
                        }
                    }
                }

                if (airPurifier.State == "on")
                {
                    if (dateTimeHelper.Now.Between(new TimeSpan(18, 0, 0), new TimeSpan(20, 0, 0)))
                    {
                        var level = Math.Min(airPurifier.Aqi / 10 + 2, 16);

                        if (airPurifier.PresetMode != AirPurifierPresetMode.Favorite ||
                            airPurifier.FavoriteLevel != level)
                        {
                            logger.LogInformation("Changing speed of {EntityId} to {Level}",
                                airPurifier.EntityId, level);

                            await airPurifier.SetFavoriteLevel(level);
                            await airPurifier.SetPresetMode(AirPurifierPresetMode.Favorite);
                        }
                    }
                    else
                    {
                        if (silentSpeedCondition.CheckFulfilled(airPurifier.Aqi <= 20))
                        {
                            if (airPurifier.PresetMode != AirPurifierPresetMode.Silent)
                            {
                                logger.LogInformation("Changing speed of {EntityId} to {Speed}",
                                    airPurifier.EntityId, AirPurifierPresetMode.Silent);

                                await airPurifier.SetPresetMode(AirPurifierPresetMode.Silent);
                            }
                        }

                        if (autoSpeedCondition.CheckFulfilled(airPurifier.Aqi > 20))
                        {
                            var level = Math.Min(airPurifier.Aqi / 10, 16);

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

        private void ClearConditions()
        {
            turnOnCondition.Reset();
            turnOffCondition.Reset();
            silentSpeedCondition.Reset();
            autoSpeedCondition.Reset();
        }
    }
}