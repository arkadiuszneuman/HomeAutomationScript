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
            Sensor.Name.BusinessLaptopWifi.GetEntityId(),
            Sensor.Name.LaptopWifi.GetEntityId(),
            Sensor.Name.DesktopComputer.GetEntityId(),
            InputNumber.Name.MinimumLightForLight.GetEntityId()
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

        public override async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            var computerLight = await connector.LoadEntityFromStates<Switch>(Switch.Name.OfficeLight.GetEntityId());
            var officeBigLight = await connector.LoadEntityFromStates<RgbLight>(RgbLight.Name.OfficeBigLight.GetEntityId());
            var officeSmallLight = await connector.LoadEntityFromStates<RgbLight>(RgbLight.Name.OfficeSmallLight.GetEntityId());
            var switchLightOn = await ShouldLightBeSwitchedOn();
            
            await officeBigLight.Turn(switchLightOn);
            await officeSmallLight.Turn(switchLightOn);
            await computerLight.Turn(switchLightOn);
        }

        private async Task<bool> ShouldLightBeSwitchedOn()
        {
            var laptopEthernet = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.LaptopEthernet.GetEntityId());
            var businessLaptop = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.BusinessLaptopWifi.GetEntityId());
            var laptopWifi = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.LaptopWifi.GetEntityId());
            var computer = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.DesktopComputer.GetEntityId());
            var sunlight = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());
            var minimumLightForLight = await InputNumber.LoadFromEntityId(connector, InputNumber.Name.MinimumLightForLight);

            if (laptopEthernet.State != "on" && businessLaptop.State != "on" && computer.State != "on" && laptopWifi.State != "on") 
                return false;
            
            if (!int.TryParse(sunlight.State, out var result))
                return false;
                
            var minimumSunState = minimumLightForLight.Value;
            if (dateTimeHelper.Now.TimeOfDay > new TimeSpan(12, 0, 0))
                minimumSunState = minimumLightForLight.Value - 5;

            return result <= minimumSunState;

        }
    }
}