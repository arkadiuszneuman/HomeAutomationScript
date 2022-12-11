using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using Humanizer;

namespace AutomationRunner.Core.Entities;

public record AqaraP1 : Sensor
{
    public enum Sensors
    {
        [EntityId("binary_sensor.buda_occupancy")]
        Occupancy,
        [EntityId("sensor.buda_battery")]
        Battery,
        [EntityId("sensor.buda_device_temperature")]
        Temperature,
        [EntityId("sensor.buda_illuminance_lux")]
        Lux
    }
    
    public new enum Name
    {
        [EntityId("sensor.buda_illuminance_lux")]
        Hut
    }

    public static IEnumerable<string> GetAllSensors() => Enum.GetValues<Sensors>().Select(x => x.GetEntityId());
    
    public static async Task<AqaraP1> LoadFromEntityId(HomeAssistantConnector entityLoader, Name cubeName)
    {
        return await entityLoader.LoadEntityFromStates<AqaraP1>(cubeName.GetEntityId());
    }
    
   
    public int Battery
    {
        get
        {
            if (!Attributes.ContainsKey("battery"))
                return default;

            var attributeValue = Attributes["battery"].ToString();

            if (attributeValue is null)
                return default;
            
            return int.TryParse(attributeValue, out var value) ? value : default;
        }
    }
    
    public int Temperature
    {
        get
        {
            if (!Attributes.ContainsKey("device_temperature"))
                return default;

            var attributeValue = Attributes["device_temperature"].ToString();

            if (attributeValue is null)
                return default;
            
            return int.TryParse(attributeValue, out var value) ? value : default;
        }
    }
    
    public int Illuminance
    {
        get
        {
            if (!Attributes.ContainsKey("illuminance"))
                return default;

            var attributeValue = Attributes["illuminance"].ToString();

            if (attributeValue is null)
                return default;
            
            return int.TryParse(attributeValue, out var value) ? value : default;
        }
    }
    
    public bool Occupancy
    {
        get
        {
            if (!Attributes.ContainsKey("occupancy"))
                return default;

            var attributeValue = Attributes["occupancy"].ToString();

            if (attributeValue is null)
                return default;
            
            return bool.TryParse(attributeValue, out var value) ? value : default;
        }
    }
}