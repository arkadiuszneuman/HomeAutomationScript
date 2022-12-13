using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific;

public class RelaysLights : BaseAutomation, IEntitiesStateAutomation
{
    private readonly HomeAssistantConnector connector;

    public IEnumerable<string> EntityNames => new[]
    {
        Switch.Name.KidsRelay.GetEntityId(),
        Switch.Name.OfficeRelay.GetEntityId()
    };

    private static readonly Dictionary<string, string> _dictionary = new()
    {
        { Switch.Name.OfficeRelay.GetEntityId(), RgbLight.Name.OfficeBigLight.GetEntityId() },
        { Switch.Name.KidsRelay.GetEntityId(), Light.Name.KidsLight.GetEntityId() },
    };

    public RelaysLights(HomeAssistantConnector connector)
    {
        this.connector = connector;
    }

    public override Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        return Task.FromResult(oldStateBaseEntity.State != newStateBaseEntity.State);
    }

    public override async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        if (_dictionary.ContainsKey(oldStateBaseEntity.EntityId))
        {
            var light = await connector.LoadEntityFromStates<RgbLight>(_dictionary[oldStateBaseEntity.EntityId]);
            await light.Turn(newStateBaseEntity.State == "on", Color.FromArgb(255, 255, 182), 100, TimeSpan.FromSeconds(1));
        }
    }
}