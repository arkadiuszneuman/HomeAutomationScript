using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class GoodnightScene : IScene
    {
        private readonly HomeAssistantConnector connector;

        public string Name => "scene.dobranoc";

        public GoodnightScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var lightsToSwitchOn = await Light.LoadFromEntitiesId(connector, Light.Name.Halogen2, Light.Name.Halogen3);
            var allLights = await Light.LoadAllLights(connector, Light.Name.Halogen2, Light.Name.Halogen3);
            var allRgbLights = await RgbLight.LoadAllLights(connector);
            var allSwitches = await Switch.LoadAllLights(connector);
            var stairsLight = await InputNumber.LoadFromEntityId(connector, InputNumber.Name.StairsMinimumBrightness);
            var mediaPlayers = await MediaPlayer.LoadAll(connector, MediaPlayer.Name.Denon);
            //var denon = await MediaPlayer.LoadFromEntityId(connector, MediaPlayer.Name.Denon);
            var cover = await Cover.LoadFromEntityId(connector, Cover.Name.Salon);

            await lightsToSwitchOn.TurnOnAll();
            await stairsLight.SetValue(30);

            await cover.CloseCover();
            await mediaPlayers.TurnOffAll();
            await allLights.TurnOffAll();
            await allRgbLights.TurnOffAll();
            await allSwitches.TurnOffAll();

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            await allRgbLights.TurnOffAll();
            //await denon.TurnOff();

            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

            await lightsToSwitchOn.TurnOffAll();
            await stairsLight.SetValue(0);

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            await allLights.TurnOffAll();
            await allRgbLights.TurnOffAll();
            await lightsToSwitchOn.TurnOffAll();
            await stairsLight.SetValue(0);
        }
    }
}
