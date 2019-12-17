﻿using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public enum AirHumidifierSpeed
    {
        High,
        Medium,
        Silent,
        Auto
    }

    public class XiaomiAirHumidifier : BaseEntity
    {
        private HomeAssistantConnector EntityLoader { get; set; }

        public int Humidity => GetAttributeValue<int>(nameof(Humidity));
        public int Temperature => GetAttributeValue<int>(nameof(Temperature));
        public int? Depth => GetAttributeValue<int>(nameof(Depth));
        public AirHumidifierSpeed Speed => GetAttributeValue<AirHumidifierSpeed>(nameof(Speed));

        public static async Task<XiaomiAirHumidifier> LoadFromEntityId(HomeAssistantConnector entityLoader, string entityId)
        {
            var deserializedObject = await entityLoader.LoadEntityFromStates<XiaomiAirHumidifier>(entityId);
            deserializedObject.EntityLoader = entityLoader;
            return deserializedObject;
        }

        public async Task TurnOn()
        {
            await EntityLoader.SendService("fan.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await EntityLoader.SendService("fan.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SetSpeed(AirHumidifierSpeed speed)
        {
            await EntityLoader.SendService("fan.set_speed", new SetSpeedService(EntityId, speed.ToString()));
        }
    }
}
