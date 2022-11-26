using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using Humanizer;

namespace AutomationRunner.Core.Entities;

public record Cube : Sensor
{
    public enum CubeAction
    {
        RotateRight,
        RotateLeft,
        Tap,
        Shake,
        Flip90,
        Flip180,
        Slide,
        Wakeup
    }
    
    private enum Sensors
    {
        [EntityId("sensor.cube_action")]
        Action,
        [EntityId("sensor.cube_action_angle")]
        ActionAngle,
        [EntityId("sensor.cube_action_from_side")]
        ActionAngleFromSide,
        [EntityId("sensor.cube_action_side")]
        ActionSide,
        [EntityId("sensor.cube_action_to_side")]
        ActionToSide,
        [EntityId("sensor.cube_side")]
        Side
    }
    
    public enum Name
    {
        [EntityId("sensor.cube_action")]
        Cube
    }

    public static IEnumerable<string> GetAllCubeSensors() => Enum.GetValues<Sensors>().Select(x => x.GetEntityId());
    
    public static async Task<Cube> LoadFromEntityId(HomeAssistantConnector entityLoader, Name cubeName)
    {
        return await entityLoader.LoadEntityFromStates<Cube>(cubeName.GetEntityId());
    }
    
    public CubeAction Action
    {
        get
        {
            if (!Attributes.ContainsKey("action"))
                return default;

            var attributeValue = Attributes["action"].ToString().Pascalize();
            
            if (attributeValue is null)
                return default;
            
            return Enum.TryParse(attributeValue, out CubeAction action) ? action : default;
        }
    }

    public decimal? Angle
    {
        get
        {
            if (!Attributes.ContainsKey("angle"))
                return default;

            var attributeValue = Attributes["angle"].ToString();

            if (attributeValue is null)
                return default;
            
            return decimal.TryParse(attributeValue, out var value) ? value : default;
        }
    }
    
    public int? Side
    {
        get
        {
            if (!Attributes.ContainsKey("side"))
                return default;

            var attributeValue = Attributes["side"].ToString().Pascalize();

            if (attributeValue is null)
                return default;
            
            return int.TryParse(attributeValue, out var value) ? value : default;
        }
    }
    
    public int? FromSide
    {
        get
        {
            if (!Attributes.ContainsKey("from_side"))
                return default;

            var attributeValue = Attributes["from_side"].ToString().Pascalize();

            if (attributeValue is null)
                return default;
            
            return int.TryParse(attributeValue, out var value) ? value : default;
        }
    }
}