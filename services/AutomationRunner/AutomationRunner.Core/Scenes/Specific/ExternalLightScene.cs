using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class ExternalLightScene : IScene
    {
        private readonly HomeAssistantConnector connector;

        public string Name => "scene.swiatlo_zewnetrzne";

        public ExternalLightScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var externalLight = await Light.LoadFromEntityId(connector, Light.Name.ExternalLight);
            var gardenLight = await Switch.LoadFromEntityId(connector, Switch.Name.GardenLight);

            await externalLight.TurnOn();
            await gardenLight.TurnOn();
        }
    }
}