using System;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Fan.Bravia
{
    public class BraviaSwitchOnLedLightsAutomation : IEntityStateAutomation, ITimeUpdate
    {
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        public string EntityName => MediaPlayer.Name.Tv.GetEntityId();
        public TimeSpan UpdateEvery => TimeSpan.FromMinutes(5);

        public BraviaSwitchOnLedLightsAutomation(HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            var oldState = MediaPlayer.CreateBasedOnBaseEntity(oldStateBaseEntity);
            var newState = MediaPlayer.CreateBasedOnBaseEntity(newStateBaseEntity);

            if (oldState.State == "off" && newState.State == "on")
                await Update();
        }

        public async Task Update()
        {
            var sun = await Sun.Load(connector, dateTimeHelper);
            var mushroom = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.Mushroom);
            var tvleds = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.TvLEDs);

            if (sun.AfterSunset(TimeSpan.FromMinutes(30)))
            {
                await tvleds.TurnOnStandardWhite();
                await mushroom.TurnOnWithRandomColor();
            }
        }
    }
}