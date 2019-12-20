﻿using AutomationRunner.Core.Common;
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
    public class Light : BaseEntity
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
            ExternalLight
        }

        public static async Task<Light> LoadFromEntityId(HomeAssistantConnector entityLoader, Name lightName)
        {
            return await entityLoader.LoadEntityFromStates<Light>(lightName.GetEntityId());
        }

        public static IEnumerable<Task<Light>> LoadFromEntitiesId(HomeAssistantConnector connector, params Name[] lightNames)
        {
            foreach (var lightName in lightNames)
                yield return LoadFromEntityId(connector, lightName);
        }

        public static IEnumerable<Task<Light>> LoadAllLights(HomeAssistantConnector connector, params Name[] except)
        {
            foreach (Name lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                yield return LoadFromEntityId(connector, lightName);
        }

        public async Task TurnOn()
        {
            await Connector.SendService("light.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("light.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}
