using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities;
using AutomationRunner.Core.Scenes.Specific;

namespace AutomationRunner.Core.Automations.Specific;

public class CubeAutomation : BaseAutomation, IEntitiesStateAutomation
{
    private readonly HomeAssistantConnector connector;
    private readonly StandardLightScene standardLightScene;
    private readonly OutHomeScene outHomeScene;
    private readonly GoodnightScene goodnightScene;
    private readonly WatchTvScene watchTvScene;
    private readonly IDateTimeHelper dateTimeHelper;
    private Cube? previousCubeState;
    private DateTime? startingGoodNight;

    public CubeAutomation(HomeAssistantConnector connector,
        StandardLightScene standardLightScene,
        OutHomeScene outHomeScene,
        GoodnightScene goodnightScene,
        WatchTvScene watchTvScene,
        IDateTimeHelper dateTimeHelper)
    {
        this.connector = connector;
        this.standardLightScene = standardLightScene;
        this.outHomeScene = outHomeScene;
        this.goodnightScene = goodnightScene;
        this.watchTvScene = watchTvScene;
        this.dateTimeHelper = dateTimeHelper;
    }

    public IEnumerable<string> EntityNames => Cube.GetAllCubeSensors();
    public override bool Enabled => false;

    public override async Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        var cube  = await Cube.LoadFromEntityId(connector, Cube.Name.Cube);

        if (previousCubeState is null)
            return true;
        
        return previousCubeState.Action != cube.Action ||
               previousCubeState.Angle != cube.Angle ||
               previousCubeState.Side != cube.Side ||
               previousCubeState.FromSide != cube.FromSide;
    }

    public override async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        var cube = await Cube.LoadFromEntityId(connector, Cube.Name.Cube);

        if (cube.Action == Cube.CubeAction.Tap)
            await standardLightScene.ActivateAsync();

        if (cube.Action == Cube.CubeAction.Shake)
            await outHomeScene.ActivateAsync();

        if (cube.Action == Cube.CubeAction.Flip90)
        {
            if (cube.FromSide == 0 && cube.Side == 2)
            {
                startingGoodNight = dateTimeHelper.UtcNow;
            }
        }

        if (cube.Action == Cube.CubeAction.RotateLeft)
        {
            if (dateTimeHelper.UtcNow - startingGoodNight < TimeSpan.FromSeconds(30))
            {
                await goodnightScene.ActivateAsync();
                startingGoodNight = null;
            }
        }
        
        if (cube.Action == Cube.CubeAction.RotateRight)
        {
            if (dateTimeHelper.UtcNow - startingGoodNight < TimeSpan.FromSeconds(30))
            {
                await watchTvScene.ActivateAsync();
                startingGoodNight = null;
            }
        }

        previousCubeState = cube;
    }
}