using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Office
{
    public class OfficeLightAutomation : BaseAutomation, IEntitiesStateAutomation
    {
        private readonly HomeAssistantConnector connector;
        private readonly IDateTimeHelper dateTimeHelper;

        public IEnumerable<string> EntityNames => new[]
        {
            Sensor.Name.Sunlight.GetEntityId(),
            Sensor.Name.LaptopEthernet.GetEntityId(),
            Sensor.Name.BusinessLaptopWifi.GetEntityId()
        };

        public OfficeLightAutomation(HomeAssistantConnector connector,
            IDateTimeHelper dateTimeHelper)
        {
            this.connector = connector;
            this.dateTimeHelper = dateTimeHelper;
        }

        public override async Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            if (oldStateBaseEntity.State != newStateBaseEntity.State)
            {
                var autoOfficeLight = await connector.LoadEntityFromStates<InputBoolean>(InputBoolean.Name.AutomaticOfficeLight.GetEntityId());

                return autoOfficeLight.IsSwitchedOn();
            }

            return false;
        }

        public override async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            var computerLight = await connector.LoadEntityFromStates<Switch>(Switch.Name.OfficeLight.GetEntityId());
            var switchLightOn = await ShouldLightBeSwitchedOn();
            await computerLight.Turn(switchLightOn);
        }

        private async Task<bool> ShouldLightBeSwitchedOn()
        {
            var laptopEthernet = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.LaptopEthernet.GetEntityId());
            var businessLaptop = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.BusinessLaptopWifi.GetEntityId());
            var sunlight = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());

            if (laptopEthernet.State == "on" || businessLaptop.State == "on")
            {
                if (int.TryParse(sunlight.State, out var result))
                {
                    var minimumSunState = 15;
                    if (dateTimeHelper.Now.TimeOfDay > new TimeSpan(12, 0, 0))
                        minimumSunState = 11;
                    
                    if (result <= minimumSunState)
                        return true;
                }
            }

            return false;
        }
    }
}