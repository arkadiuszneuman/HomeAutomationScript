using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using Humanizer;

namespace AutomationRunner.Core.Entities;

public record Remote : Sensor
{
    public enum RemoteAction
    {
        Toggle,
        ArrowRightClick,
        ArrowRightHold,
        ArrowRightRelease,
        ArrowLeftClick,
        ArrowLeftHold,
        ArrowLeftRelease,
        BrightnessUpClick,
        BrightnessUpHold,
        BrightnessUpRelease,
        BrightnessDownClick,
        BrightnessDownHold,
        BrightnessDownRelease
    }
    
    public new enum Name
    {
        [EntityId("update.pilot")]
        Remote
    }

    
    public static async Task<Remote> LoadFromEntityId(HomeAssistantConnector entityLoader, Name cubeName)
    {
        return await entityLoader.LoadEntityFromStates<Remote>(cubeName.GetEntityId());
    }
    
    public RemoteAction Action
    {
        get
        {
            if (!Attributes.ContainsKey("action"))
                return default;

            var attributeValue = Attributes["action"].ToString().Pascalize();
            
            if (attributeValue is null)
                return default;
            
            return Enum.TryParse(attributeValue, out RemoteAction action) ? action : default;
        }
    }
}