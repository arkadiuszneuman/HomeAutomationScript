using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class OutHomeScene : BaseScene
    {
        public override string Name => "scene.wyjscie_z_domu";

        public OutHomeScene(HomeAssistantConnector connector) : base(connector)
        {
        }

        public override async Task Activated(CancellationToken cancellationToken = default)
        {
            var lightsToSwitchOn = await Light.LoadFromEntitiesId(Connector, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.ExternalLight);
            var allLights = await Light.LoadAllLights(Connector, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.ExternalLight);
            var allRgbLights = await RgbLight.LoadAllLights(Connector);
            var allSwitches = await Switch.LoadAllLights(Connector, Switch.Name.GardenLight);
            var stairsLight = await InputNumber.LoadFromEntityId(Connector, InputNumber.Name.StairsMinimumBrightness);
            var mediaPlayers = await MediaPlayer.LoadAll(Connector);
            var cover = await Cover.LoadFromEntityId(Connector, Cover.Name.Salon);
            var gardenLight = await Switch.LoadFromEntityId(Connector, Switch.Name.GardenLight);

            var sunlight = await Connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());
            if (int.TryParse(sunlight.State, out var result) && result < 4)
            {
                await lightsToSwitchOn.TurnOnAll();
                await gardenLight.TurnOn();
                await cover.CloseCover();
            }
            
            await allLights.TurnOffAll();
            await allRgbLights.TurnOffAll();
            await allSwitches.TurnOffAll();
            await stairsLight.SetValue(0);

            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            
            await mediaPlayers.TurnOffAll();

            await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);

            await lightsToSwitchOn.TurnOffAll();
            await gardenLight.TurnOff();
        }
    }
}