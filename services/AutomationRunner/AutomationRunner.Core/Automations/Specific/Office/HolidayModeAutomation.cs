using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using Microsoft.Extensions.Logging;

namespace AutomationRunner.Core.Automations.Specific.Office;

public class HolidayModeAutomation : BaseAutomation, IEntitiesStateAutomation, ITimeUpdate
{
    private readonly HomeAssistantConnector connector;
    private readonly IDateTimeHelper dateTimeHelper;
    private readonly ILogger<HolidayModeAutomation> logger;

    public TimeSpan UpdateEvery => TimeSpan.FromMinutes(1);

    public IEnumerable<string> EntityNames => new[]
    {
        Sensor.Name.Sunlight.GetEntityId(),
        InputBoolean.Name.HolidayMode.GetEntityId()
    };

    public HolidayModeAutomation(HomeAssistantConnector connector,
        IDateTimeHelper dateTimeHelper,
        ILogger<HolidayModeAutomation> logger)
    {
        this.connector = connector;
        this.dateTimeHelper = dateTimeHelper;
        this.logger = logger;
    }

    public override async Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        if (oldStateBaseEntity.State != newStateBaseEntity.State)
        {
            var holidayMode = await connector.LoadEntityFromStates<InputBoolean>(InputBoolean.Name.HolidayMode.GetEntityId());

            return holidayMode.IsSwitchedOn();
        }

        return false;
    }

    public override async Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        await Update();
    }

    public async Task Update()
    {
        var holidayMode = await connector.LoadEntityFromStates<InputBoolean>(InputBoolean.Name.HolidayMode.GetEntityId());

        if (holidayMode.IsSwitchedOn())
        {
            var cover = await Cover.LoadFromEntityId(connector, Cover.Name.Salon);

            var sunlight = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());
            var allLights = await Light.LoadFromEntitiesId(connector, Light.Name.Halogen1, Light.Name.Halogen3);
            if (int.TryParse(sunlight.State, out var result))
            {
                if (result < 4)
                {
                    if (cover.Position > 0)
                    {
                        await cover.CloseCover();
                    }
                }

                if (result >= 4 && dateTimeHelper.Now.TimeOfDay > new TimeSpan(8, 0, 0))
                {
                    if (cover.Position < 100)
                    {
                        await cover.OpenCover();
                    }
                }

                if (result < 4)
                {
                    if (dateTimeHelper.Now.TimeOfDay < new TimeSpan(8, 0, 0) || dateTimeHelper.Now.TimeOfDay > new TimeSpan(22, 00, 0))
                        await allLights.TurnOffAll();
                    else 
                        await allLights.TurnOnAll();
                }
                else
                {
                    await allLights.TurnOffAll();
                }
            }
        }
    }
}