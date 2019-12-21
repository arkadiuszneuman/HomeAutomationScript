using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
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
            var stairs = await InputNumber.LoadFromEntityId(connector, InputNumber.Name.StairsMinimumBrightness);

            var switches = Switch.LoadFromEntitiesId(connector, Switch.Name.ChildLight, 
                Switch.Name.ChristmasTree, Switch.Name.SalonLights);

            await switches.TurnOnAll();
            await stairs.SetValueBasedOnTvState();
            await mushroom.TurnOnWithRandomColor();
        }
    }
}
