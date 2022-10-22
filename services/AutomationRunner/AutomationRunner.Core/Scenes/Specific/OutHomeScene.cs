using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class OutHomeScene : IScene
    {
        private readonly HomeAssistantConnector connector;

        public string Name => "scene.wyjscie_z_domu";

        public OutHomeScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var lightsToSwitchOn = await Light.LoadFromEntitiesId(connector, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.ExternalLight);
            var allLights = await Light.LoadAllLights(connector, Light.Name.Halogen2, Light.Name.Halogen3, Light.Name.ExternalLight);
            var allRgbLights = await RgbLight.LoadAllLights(connector);
            var allSwitches = await Switch.LoadAllLights(connector, Switch.Name.GardenLight);
            var stairsLight = await InputNumber.LoadFromEntityId(connector, InputNumber.Name.StairsMinimumBrightness);
            var mediaPlayers = await MediaPlayer.LoadAll(connector);
            var cover = await Cover.LoadFromEntityId(connector, Cover.Name.Salon);
            var gardenLight = await Switch.LoadFromEntityId(connector, Switch.Name.GardenLight);

            var sunlight = await connector.LoadEntityFromStates<Sensor>(Sensor.Name.Sunlight.GetEntityId());
            if (int.TryParse(sunlight.State, out var result) && result < 4)
            {
                await lightsToSwitchOn.TurnOnAll();
                await gardenLight.TurnOn();
                await cover.CloseCover();
            }

            await mediaPlayers.TurnOffAll();
            await allLights.TurnOffAll();
            await allRgbLights.TurnOffAll();
            await allSwitches.TurnOffAll();
            await stairsLight.SetValue(0);

            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);

            await lightsToSwitchOn.TurnOffAll();
            await gardenLight.TurnOff();
        }
    }
}