﻿using AutomationRunner.Automations.Helpers;
using AutomationRunner.Common;
using AutomationRunner.Common.Connector;
using AutomationRunner.Common.Extensions;
using AutomationRunner.Entities;
using AutomationRunner.Entities.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Specific.Fan.AirHumidifier
{
    public class AirHumidifierWaterLevelNotification : IAutomation
    {
        private const double percentToInform = 20;

        private readonly ILogger<AirHumidifierAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly NotificationPushService notificationPushService;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ConditionHelper notifyCondition;

        public Task<XiaomiAirHumidifier> LoadAirHumidifierEntity() => XiaomiAirHumidifier.LoadFromEntityId(connector, "fan.air_humidifier");

        public AirHumidifierWaterLevelNotification(
            ILogger<AirHumidifierAutomations> logger,
            HomeAssistantConnector connector,
            AutomationHelpersFactory automationHelpersFactory,
            NotificationPushService notificationPushService,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.notificationPushService = notificationPushService;
            this.dateTimeHelper = dateTimeHelper;
            this.notifyCondition = automationHelpersFactory
                .GetConditionHelper()
                .For(TimeSpan.Zero);
        }

        public async Task Update()
        {
            var airHumidifer = await LoadAirHumidifierEntity();

            if (dateTimeHelper.Now.Between(new TimeSpan(23, 0, 0), new TimeSpan(8, 0, 0)))
            {
                notifyCondition.Reset();
                return;
            }

            var waterLevelPercent = GetPercentDepth(airHumidifer.Attributes.Depth);

            if (notifyCondition.CheckFulfilled(waterLevelPercent < percentToInform))
            {
                logger.LogInformation("Sending notification about water level. Actual: {0}%", waterLevelPercent);
                await notificationPushService.PushNotification($"Zbyt mało wody: {waterLevelPercent}%");
            }
        }

        private double GetPercentDepth(int depth)
        {
            return depth / 1.2;
        }
    }
}
