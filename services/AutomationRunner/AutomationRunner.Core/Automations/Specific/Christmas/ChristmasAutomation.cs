using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Christmas
{
    public class ChristmasAutomation : BaseAutomation, ITimeUpdate
    {
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;
        private bool doneToday;
        public TimeSpan UpdateEvery => TimeSpan.FromMinutes(5);

        public static TimeSpan SwitchOffOn = new(21, 0, 0);

        public ChristmasAutomation(HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }
        
        public override Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            return Update();
        }

        
        public async Task Update()
        {
            if (Consts.Christmas)
            {
                var sun = await Sun.Load(connector, dateTimeHelper);
                var gardenLight = await Switch.LoadFromEntityId(connector, Switch.Name.GardenLight);
                
                if (!doneToday)
                {
                    if (sun.AfterSunset(TimeSpan.FromMinutes(30)))
                    {
                        doneToday = true;
                        await gardenLight.TurnOn();
                    }
                }
                else if (dateTimeHelper.UtcNow > sun.NextRisingUtc)
                {
                    doneToday = false;
                }
                else if (dateTimeHelper.UtcNow.TimeOfDay > SwitchOffOn)
                {
                    await gardenLight.TurnOff();
                }
            }
        }
    }
}