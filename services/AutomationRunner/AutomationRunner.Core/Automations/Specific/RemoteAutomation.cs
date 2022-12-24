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
    private readonly ScenesActivator scenesActivator;
    
    public IEnumerable<string> EntityNames => new[] { Remote.Name.Remote.GetEntityId() };

    private static CancellationTokenSource cancellationTokenSource = new();
    private static bool isStandardLightScene = false;

    public RemoteAutomation(HomeAssistantConnector connector, StandardLightScene standardLightScene,
        OutHomeScene outHomeScene, GoodnightScene goodnightScene, WatchTvScene watchTvScene, ScenesActivator scenesActivator)
    {
        this.connector = connector;
        this.standardLightScene = standardLightScene;
        this.outHomeScene = outHomeScene;
        this.goodnightScene = goodnightScene;
        this.watchTvScene = watchTvScene;
        this.scenesActivator = scenesActivator;
    }

    public override async Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity)
    {
        var remote = await Remote.LoadFromEntityId(connector, Remote.Name.Remote);
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        
        switch (remote.Action)
        {
            case Remote.RemoteAction.Toggle:
                var task = isStandardLightScene
                    ? watchTvScene.ActivateAsync(cancellationTokenSource.Token)
                    : standardLightScene.ActivateAsync(cancellationTokenSource.Token);

                isStandardLightScene = !isStandardLightScene;

                await task;
                
                break;
            
            case Remote.RemoteAction.BrightnessUpClick:
                await outHomeScene.ActivateAsync(cancellationTokenSource.Token);
                break;
            
            case Remote.RemoteAction.BrightnessDownClick:
                await goodnightScene.ActivateAsync(cancellationTokenSource.Token);
                break;
        }
    }
}