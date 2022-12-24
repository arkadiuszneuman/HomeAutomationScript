using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Bravia
{
    public class BraviaSwitchOnLedLightsAutomation : IEntitiesStateAutomation
    {
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        public bool Enabled => true;

        private DateTime? lastUpdateAfterSunset;

        public IEnumerable<string> EntityNames =>
            new[]
            {
                MediaPlayer.Name.Tv.GetEntityId(),
                Sun.EntityId
            };

        public BraviaSwitchOnLedLightsAutomation(HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            if (oldStateBaseEntity.EntityId == MediaPlayer.Name.Tv.GetEntityId())
            {
                var oldState = MediaPlayer.CreateBasedOnBaseEntity(oldStateBaseEntity);
                var newState = MediaPlayer.CreateBasedOnBaseEntity(newStateBaseEntity);

                if (oldState.State == "off" && newState.State == "on")
                    await TurnOn();
            }
            else if (oldStateBaseEntity.EntityId == Sun.EntityId)
            {
                if (lastUpdateAfterSunset == null ||
                    lastUpdateAfterSunset.Value.Date != DateTime.UtcNow.Date)
                    await TurnOn();
            }
        }

        private async Task TurnOn()
        {
            var mushroom = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.Mushroom);
            var tvleds = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.TvLEDs);
            var sun = await Sun.Load(connector, dateTimeHelper);
            var tv = await MediaPlayer.LoadFromEntityId(connector, MediaPlayer.Name.Tv);

            if (tv.State == "on" &&
                sun.AfterSunset(-TimeSpan.FromMinutes(30)))
            {
                lastUpdateAfterSunset = DateTime.UtcNow;
                await tvleds.TurnOnStandardWhite();
                await mushroom.TurnOnWithRandomColor();
            }
        }
    }
}