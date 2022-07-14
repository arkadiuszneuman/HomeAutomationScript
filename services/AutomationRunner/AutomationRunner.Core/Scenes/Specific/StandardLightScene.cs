using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

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

            var lightsToSwitchOff = await Light.LoadAllLights(connector, Light.Name.Halogen1, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.Halogen4);
            var lights = await Light.LoadFromEntitiesId(connector, Light.Name.Halogen1, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.Halogen4);

            await tvleds.TurnOnStandardWhite();
            await lights.TurnOnAll();
            await lightsToSwitchOff.TurnOffAll();
            await stairs.SetValueBasedOnTvState();
            await mushroom.TurnOnWithRandomColor();
        }
    }
}