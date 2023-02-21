using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific;

public class WatchTvScene : BaseScene
{
    public override string Name => "scene.ogladanie_tv";

    public WatchTvScene(HomeAssistantConnector connector) : base(connector)
    {
    }

    public override async Task Activated(CancellationToken cancellationToken = default)
    {
        var mushroom = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Mushroom);
        var halogens = await Light.LoadFromEntitiesId(Connector, Light.Name.Halogen1, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.Halogen4);
        var halogen5 = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Halogen5);
        var halogen6 = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Halogen6);
        var tvleds = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.TvLEDs);

        await tvleds.TurnOnStandardWhite();
        await halogen5.TurnOn();
        await halogen6.TurnOn();
        await mushroom.TurnOnWithRandomColor();
        await halogens.TurnOffAll();
        
        if (Consts.Christmas)
        {
            var christmasLight = await Switch.LoadFromEntityId(Connector, Switch.Name.ChristmasTree);
            await christmasLight.TurnOn();
        }
    }
}