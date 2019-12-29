using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(Math.Min(Math.Min(forTurnOnTime, forTurnOffTime), speedChangeForTime));
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
            this.turnOffCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOffTime));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(forTurnOnTime));

            this.silentSpeedCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(speedChangeForTime));

            this.autoSpeedCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.FromMinutes(speedChangeForTime));
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

            if (dateTimeHelper.Now.Between(new TimeSpan(8, 0, 0), new TimeSpan(19, 0, 0)))
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
                        logger.LogInformation("Turning on {EntityId}, because aqi is lower or equal than {TurningOffValue} for {ForTime} minutes",
                            airPurifier.EntityId, turningOffValue, forTurnOnTime);
                        await airPurifier.TurnOn();

                        if (airPurifier.Speed != AirPurifierSpeed.Silent)
                        {
                            logger.LogInformation("Changing speed of {EntityId} to {Speed} after turning on",
                                   airPurifier.EntityId, AirPurifierSpeed.Silent);

                            await airPurifier.SetSpeed(AirPurifierSpeed.Silent);
                        }
                    }
                }

                if (airPurifier.State == "on")
                {
                    if (dateTimeHelper.Now.Between(new TimeSpan(18, 0, 0), new TimeSpan(20, 0, 0)))
                    {
                        var level = Math.Min((airPurifier.Aqi / 10) + 2, 16);

                        if (airPurifier.Speed != AirPurifierSpeed.Favorite ||
                            airPurifier.FavoriteLevel != level)
                        {
                            logger.LogInformation("Changing speed of {EntityId} to {Level}",
                                    airPurifier.EntityId, level);

                            await airPurifier.SetLevel(level);
                            await airPurifier.SetSpeed(AirPurifierSpeed.Favorite);
                        }
                    }
                    else
                    {
                        if (silentSpeedCondition.CheckFulfilled(airPurifier.Aqi <= 20))
                        {
                            if (airPurifier.Speed != AirPurifierSpeed.Silent)
                            {
                                logger.LogInformation("Changing speed of {EntityId} to {Speed}",
                                       airPurifier.EntityId, AirPurifierSpeed.Silent);

                                await airPurifier.SetSpeed(AirPurifierSpeed.Silent);
                            }
                        }

                        if (autoSpeedCondition.CheckFulfilled(airPurifier.Aqi > 20))
                        {
                            if (airPurifier.Speed != AirPurifierSpeed.Auto)
                            {
                                logger.LogInformation("Changing speed of {EntityId} to {Speed}",
                                    airPurifier.EntityId, AirPurifierSpeed.Auto);
                                await airPurifier.SetSpeed(AirPurifierSpeed.Auto);
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
