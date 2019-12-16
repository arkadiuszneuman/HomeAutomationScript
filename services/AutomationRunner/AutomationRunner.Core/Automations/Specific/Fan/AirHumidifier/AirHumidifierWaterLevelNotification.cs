using AutomationRunner.Core.Automations.Helpers;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using AutomationRunner.Core.Entities.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirHumidifier
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
                logger.LogInformation("Sending notification about water level. Actual: {WaterLevelPercent}%", waterLevelPercent);
                await notificationPushService.PushNotification($"Zbyt mało wody: {waterLevelPercent:N2}%");
            }
        }

        private double GetPercentDepth(int depth)
        {
            return depth / 1.2;
        }
    }
}
