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
    public class GoodnightScene : BaseScene
    {
        public override string Name => "scene.dobranoc";

        public GoodnightScene(HomeAssistantConnector connector) : base(connector)
        {
        }

        public override async Task Activated(CancellationToken cancellationToken = default)
        {
            var lightsToSwitchOn = await Light.LoadFromEntitiesId(Connector, Light.Name.Halogen2, Light.Name.Halogen3);
            var allLights = await Light.LoadAllLights(Connector, Light.Name.Halogen2, Light.Name.Halogen3);
            var allRgbLights = await RgbLight.LoadAllLights(Connector);
            var allSwitches = await Switch.LoadAllLights(Connector, Switch.Name.OfficeLight);
            var officeLight = await Switch.LoadFromEntityId(Connector, Switch.Name.OfficeLight);
            var stairsLight = await InputNumber.LoadFromEntityId(Connector, InputNumber.Name.StairsMinimumBrightness);
            var mediaPlayers = await MediaPlayer.LoadAll(Connector);
            var cover = await Cover.LoadFromEntityId(Connector, Cover.Name.Salon);

            await lightsToSwitchOn.TurnOnAll();
            await stairsLight.SetValue(30);

            await cover.CloseCover();
            await mediaPlayers.TurnOffAll();
            await allLights.TurnOffAll();
            await allRgbLights.TurnOffAll();
            await allSwitches.TurnOffAll();
            await officeLight.TurnOn();

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            await allRgbLights.TurnOffAll();
            
            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            await officeLight.TurnOff();

            await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);

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
