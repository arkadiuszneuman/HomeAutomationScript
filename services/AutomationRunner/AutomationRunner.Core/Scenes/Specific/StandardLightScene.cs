using System.Drawing;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class StandardLightScene : IScene
    {
        private readonly HomeAssistantConnector connector;

        public string Name => "scene.standardowe_oswietlenie";

        public StandardLightScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var mushroom = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.Mushroom);
            var tvleds = await RgbLight.LoadFromEntityId(connector, RgbLight.Name.TvLEDs);
            var stairs = await InputNumber.LoadFromEntityId(connector, InputNumber.Name.StairsMinimumBrightness);

            var lights = Light.LoadFromEntitiesId(connector, Light.Name.Halogen1, Light.Name.Halogen4);

            await tvleds.TurnOn(Color.FromArgb(255, 255, 163, 72), brightnessPercent: 100);
            await lights.TurnOnAll();
            await stairs.SetValueBasedOnTvState();
            await mushroom.TurnOnWithRandomColor();
        }
    }
}
