using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using AutomationRunner.Core.Scenes;
using AutomationRunner.Core.Scenes.Specific;

namespace AutomationRunner.Core.Automations.Specific;

public class RemoteAutomation : BaseAutomation, IEntitiesStateAutomation
{
    private readonly HomeAssistantConnector connector;
    private readonly StandardLightScene standardLightScene;
    private readonly OutHomeScene outHomeScene;
    private readonly GoodnightScene goodnightScene;
    private readonly WatchTvScene watchTvScene;
    
    public IEnumerable<string> EntityNames => new[] { Remote.Name.Remote.GetEntityId() };

    private static CancellationTokenSource cancellationTokenSource = new();

    public RemoteAutomation(HomeAssistantConnector connector, StandardLightScene standardLightScene,
        OutHomeScene outHomeScene, GoodnightScene goodnightScene, WatchTvScene watchTvScene)
    {
        this.connector = connector;
        this.standardLightScene = standardLightScene;
        this.outHomeScene = outHomeScene;
        this.goodnightScene = goodnightScene;
        this.watchTvScene = watchTvScene;
    }

    public override async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        var remote = await Remote.LoadFromEntityId(connector, Remote.Name.Remote);
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        
        switch (remote.Action)
        {
            case Remote.RemoteAction.ArrowLeftClick:
                _ = standardLightScene.ActivateAsync(cancellationTokenSource.Token);
                break;
            case Remote.RemoteAction.ArrowRightClick:
                _ = watchTvScene.ActivateAsync(cancellationTokenSource.Token);
                break;
            case Remote.RemoteAction.ArrowRightHold:
                var rgbLights = await RgbLight.LoadAllLights(connector, RgbLight.Name.Mushroom, RgbLight.Name.TvLEDs);
                await watchTvScene.ActivateAsync(cancellationTokenSource.Token);
                _ = rgbLights.TurnOffAll();
                break;
            case Remote.RemoteAction.BrightnessUpClick:
                _ = outHomeScene.ActivateAsync(cancellationTokenSource.Token);
                break;
            case Remote.RemoteAction.BrightnessDownClick:
                _ = goodnightScene.ActivateAsync(cancellationTokenSource.Token);
                break;
        }
    }
}