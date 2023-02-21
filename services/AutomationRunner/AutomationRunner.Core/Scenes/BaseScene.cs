using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities.Services.Models;
using AutomationRunner.Core.Scenes.Specific;

namespace AutomationRunner.Core.Scenes;

public abstract class BaseScene : IScene
{
    protected HomeAssistantConnector Connector { get; }
    public abstract string Name { get; }

    protected BaseScene(HomeAssistantConnector connector)
    {
        Connector = connector;
    }
    
    public abstract Task Activated(CancellationToken cancellationToken = default);

    public async Task ActivateAsync(CancellationToken cancellationToken = default)
    {
        await Connector.SendServiceAsync("scene.turn_on", new EntityIdService(Name), cancellationToken);
    }
}