using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class StandardLightScene : BaseScene
    {
        public override string Name => "scene.standardowe_oswietlenie";

        public StandardLightScene(HomeAssistantConnector connector) : base(connector)
        {
        }

        public override async Task Activated(CancellationToken cancellationToken = default)
        {
            var mushroom = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Mushroom);
            var tvleds = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.TvLEDs);
            var halogen5 = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Halogen5);
            var halogen6 = await RgbLight.LoadFromEntityId(Connector, RgbLight.Name.Halogen6);
            var stairs = await InputNumber.LoadFromEntityId(Connector, InputNumber.Name.StairsMinimumBrightness);

            var lightsToSwitchOff = await Light.LoadAllLights(Connector, Light.Name.Halogen1, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.Halogen4);
            var lights = await Light.LoadFromEntitiesId(Connector, Light.Name.Halogen1, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.Halogen4);

            await tvleds.TurnOnStandardWhite();
            await halogen5.TurnOn(Color.FromArgb(42, 0, 255));
            await halogen6.TurnOn(Color.FromArgb(255, 0, 255));
            await lights.TurnOnAll();
            await lightsToSwitchOff.TurnOffAll();
            await stairs.SetValueBasedOnTvState();
            await mushroom.TurnOnWithRandomColor();
        }
    }
}