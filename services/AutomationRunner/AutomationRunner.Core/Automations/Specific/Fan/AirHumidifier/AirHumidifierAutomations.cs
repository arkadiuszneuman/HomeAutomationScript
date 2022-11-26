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
        private readonly ILogger<AirHumidifierAutomations> logger;
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;

        public string EntityName { get; } = XiaomiAirHumidifier.Name.AirHumidifier.GetEntityId();
        public TimeSpan UpdateEvery { get; } = TimeSpan.FromMinutes(1);

        public AirHumidifierAutomations(
            ILogger<AirHumidifierAutomations> logger,
            HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.logger = logger;
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
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
                await airHumidifer.TurnOff();
            }
            else
            {
                await airHumidifer.TurnOn();
                await airHumidifer.SetSpeed(AirHumidifierSpeed.Auto);
            }
        }
    }
}
