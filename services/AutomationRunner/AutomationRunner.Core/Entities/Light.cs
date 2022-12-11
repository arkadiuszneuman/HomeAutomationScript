using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

namespace AutomationRunner.Core.Entities
{
    public record Light : BaseEntity
    {
        public enum Name
        {
            [EntityId("light.halogen_1")]
            Halogen1,

            [EntityId("light.halogen_2")]
            Halogen2,

            [EntityId("light.halogen_3")]
            Halogen3,

            [EntityId("light.halogen_4")]
            Halogen4,

            [EntityId("light.lampka_zewnetrzna")]
            ExternalLight,
            
            [EntityId("light.lampa_dzieci")]
            KidsLight
        }

        public static async Task<Light> LoadFromEntityId(HomeAssistantConnector connector, Name lightName)
        {
            return await connector.LoadEntityFromStates<Light>(lightName.GetEntityId());
        }

        public static async Task<IList<Light>> LoadFromEntitiesId(HomeAssistantConnector connector, params Name[] lightNames)
        {
            var lights = new List<Light>();
            foreach (var lightName in lightNames)
                lights.Add(await LoadFromEntityId(connector, lightName));

            return lights;
        }
        
        public static async Task<IList<Light>> LoadAllLights(HomeAssistantConnector connector, params Name[] except)
        {
            var lights = new List<Light>();
            foreach (Name lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                lights.Add(await LoadFromEntityId(connector, lightName));

            return lights;
        }

        public async Task TurnOn()
        {
            await Connector.SendServiceAsync("light.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendServiceAsync("light.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
        
        public async Task Turn(bool on)
        {
            if (on)
                await TurnOn();
            else
                await TurnOff();
        }
    }
}
