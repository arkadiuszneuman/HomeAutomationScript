﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific.Office
{
    public class OfficeLightAutomation : IEntitiesStateAutomation
    {
        private readonly HomeAssistantConnector connector;

        public IEnumerable<string> EntityNames => new[]
        {
            Sensor.Name.Sunlight.GetEntityId(),
            Sensor.Name.LaptopEthernet.GetEntityId()
        };

        public OfficeLightAutomation(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
        {
            if (oldStateBaseEntity.State != newStateBaseEntity.State) 
                await Update();
        }

        private async Task Update()
        {
            var computerLight = await connector.LoadEntityFromStates<Switch>(Switch.Name.OfficeLight.GetEntityId());
            var switchLightOn = await ShouldLightBeSwitchedOn();
            await computerLight.Turn(switchLightOn);
        }

        private async Task<bool> ShouldLightBeSwitchedOn()
        {
            var laptopEthernet = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.LaptopEthernet.GetEntityId());
            var sunlight = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());

            if (laptopEthernet.State == "on")
            {
                if (int.TryParse(sunlight.State, out var result))
                {
                    if (result <= 8)
                        return true;
                }
            }

            return false;
        }
    }
}