using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;

namespace AutomationRunner.Core.Entities
{
    public record Switch : BaseEntity
    {
        public enum Name
        {
            [EntityId("switch.sonoff_1000513cd0")] ChildLight,

            [EntityId("switch.sonoff_100051420f")] ChristmasTree,

            [EntityId("switch.sonoff_1000511f63")] OfficeLight,

            [EntityId("switch.sonoff_1000541365")] GardenLight
        }

        public static async Task<Switch> LoadFromEntityId(HomeAssistantConnector connector, Name switchName)
        {
            return await connector.LoadEntityFromStates<Switch>(switchName.GetEntityId());
        }

        public static IEnumerable<Task<Switch>> LoadFromEntitiesId(HomeAssistantConnector connector,
            params Name[] switchNames)
        {
            foreach (var switchName in switchNames)
                yield return LoadFromEntityId(connector, switchName);
        }

        public static async Task<IList<Switch>> LoadAllLights(HomeAssistantConnector connector, params Name[] except)
        {
            var switches = new List<Switch>();
            foreach (var lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                switches.Add(await LoadFromEntityId(connector, lightName));

            return switches;
        }

        public Task Turn(bool on) => on ? TurnOn() : TurnOff();

        public async Task TurnOn()
        {
            await Connector.SendServiceAsync("switch.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendServiceAsync("switch.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}