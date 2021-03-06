﻿using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirHumidifier
{
    public class AirHumidifierAutomations : IEntityStateAutomation, ITimeUpdate
    {
        private const double forTime = 3;
        private const int turningOnValue = 45;
        private const int turningOffValue = 55;

        private readonly ILogger<AirHumidifierAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper turnOffCondition;
        private readonly ConditionHelper turnOnCondition;

        public string EntityName { get; } = XiaomiAirHumidifier.Name.AirHumidifier.GetEntityId();
        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(forTime);

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
                .For(TimeSpan.FromMinutes(1));

            this.turnOnCondition = automationHelpersFactory
                .GetConditionHelper()
                .Name(logger, "Air humidifier turning on")
                .For(TimeSpan.FromMinutes(10));
        }

        public Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            return Update();
        }

        public async Task Update()
        {
            var airHumidifer = await XiaomiAirHumidifier.LoadFromEntityId(connector, XiaomiAirHumidifier.Name.AirHumidifier);

            logger.LogDebug("Checking air humidifier. Humidity: {Humidity}", airHumidifer.Humidity);
            logger.LogDebug("Air humidifier current state: {State}", airHumidifer.State);

            if (dateTimeHelper.Now.Between(new TimeSpan(10, 0, 0), new TimeSpan(19, 0, 0)))
            {
                if (airHumidifer.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because of day", airHumidifer.EntityId);
                    await airHumidifer.TurnOff();
                }

                turnOnCondition.Reset();
                turnOffCondition.Reset();

                return;
            }

            if (turnOffCondition.CheckFulfilled(airHumidifer.Humidity >= turningOffValue))
            {
                if (airHumidifer.State == "on")
                {
                    logger.LogInformation("Turning off {EntityId}, because humidity is bigger than {TurningOnValue} for {ForTime} minutes",
                        airHumidifer.EntityId, turningOffValue, forTime);
                    await airHumidifer.TurnOff();
                }
            }

            if (turnOnCondition.CheckFulfilled(airHumidifer.Humidity <= turningOnValue))
            {
                if (airHumidifer.State == "off")
                {
                    logger.LogInformation("Turning on {EntityId}, because humidity is lower or equal than {TurningOnValue} for {ForTime} minutes",
                        airHumidifer.EntityId, turningOnValue, forTime);
                    await airHumidifer.TurnOn();
                }
            }

            if (airHumidifer.State == "on")
            {
                var changeHumidityInfo = airHumidifer.Humidity switch
                {
                    var humidity when humidity <= 40 => new { Change = true, Speed = AirHumidifierSpeed.High },
                    var humidity when humidity <= 50 => new { Change = true, Speed = AirHumidifierSpeed.Medium },
                    var humidity when humidity <= turningOffValue => new { Change = true, Speed = AirHumidifierSpeed.Silent },
                    _ => new { Change = false, Speed = AirHumidifierSpeed.Auto }
                };

                logger.LogDebug("Change humidity info. Change: {Change}, Speed: {Speed}", changeHumidityInfo.Change, changeHumidityInfo.Speed);

                if (changeHumidityInfo.Change && changeHumidityInfo.Speed != airHumidifer.Speed)
                {
                    logger.LogInformation("Changing speed of {EntityId} from {FromSpeed} to {ToSpeed}",
                        airHumidifer.EntityId, airHumidifer.Speed, changeHumidityInfo.Speed);
                    await airHumidifer.SetSpeed(changeHumidityInfo.Speed);
                }
            }
        }
    }
}
