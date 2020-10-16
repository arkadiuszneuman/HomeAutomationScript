using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Fan.Bravia
{
    public class BraviaSwitchOnLedLightsAutomation : IEntitiesStateAutomation
    {
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        public string EntityName => MediaPlayer.Name.Tv.GetEntityId();
        private DateTime? lastUpdateAfterSunset;

        public IEnumerable<string> EntityNames =>
            new[]
            {
                MediaPlayer.Name.Tv.GetEntityId(),
                Sun.EntityId
            };

        public TimeSpan UpdateEvery => TimeSpan.FromMinutes(5);

        public BraviaSwitchOnLedLightsAutomation(HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            var mushroom = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.Mushroom);
            var tvleds = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.TvLEDs);
            
            if (oldStateBaseEntity.EntityId == MediaPlayer.Name.Tv.GetEntityId())
            {
                var oldState = MediaPlayer.CreateBasedOnBaseEntity(oldStateBaseEntity);
                var newState = MediaPlayer.CreateBasedOnBaseEntity(newStateBaseEntity);

                if (oldState.State == "off" && newState.State == "on")
                {
                    await tvleds.TurnOnStandardWhite();
                    await mushroom.TurnOnWithRandomColor();
                }
            }
            else if (oldStateBaseEntity.EntityId == Sun.EntityId)
            {
                if (lastUpdateAfterSunset == null || 
                    lastUpdateAfterSunset.Value.Date != DateTime.UtcNow.Date)
                {
                    var sun = await Sun.Load(connector, dateTimeHelper);
                    if (sun.AfterSunset(-TimeSpan.FromMinutes(30)))
                    {
                        lastUpdateAfterSunset = DateTime.UtcNow;
                        await tvleds.TurnOnStandardWhite();
                        await mushroom.TurnOnWithRandomColor();
                    }
                }
            }
        }
    }
}