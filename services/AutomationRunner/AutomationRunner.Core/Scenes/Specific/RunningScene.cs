using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Automations.Specific.Office;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class RunningScene  : IScene
    {
        private readonly HomeAssistantConnector connector;
        public string Name => "scene.bieganie";
        
        public RunningScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }
        
        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var autoOfficeLight = await connector.LoadEntityFromStates<InputBoolean>(InputBoolean.Name.AutomaticOfficeLight.GetEntityId());
            var officeLight = await connector.LoadEntityFromStates<Switch>(Switch.Name.OfficeLight.GetEntityId());
            var treadmill = await connector.LoadEntityFromStates<Switch>(Switch.Name.ChildLight.GetEntityId());
            await autoOfficeLight.TurnOff();
            await officeLight.TurnOn();
            await treadmill.TurnOn();
        }
    }
    
    public class StopRunningScene  : IScene
    {
        private readonly HomeAssistantConnector connector;
        private readonly OfficeLightAutomation officeLightAutomation;
        public string Name => "scene.zakoncz_bieganie";
        
        public StopRunningScene(HomeAssistantConnector connector,
            OfficeLightAutomation officeLightAutomation)
        {
            this.connector = connector;
            this.officeLightAutomation = officeLightAutomation;
        }
        
        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var autoOfficeLight = await connector.LoadEntityFromStates<InputBoolean>(InputBoolean.Name.AutomaticOfficeLight.GetEntityId());
            var treadmill = await connector.LoadEntityFromStates<Switch>(Switch.Name.ChildLight.GetEntityId());
            
            await autoOfficeLight.TurnOn();
            await treadmill.TurnOff();

            await officeLightAutomation.Update(null, null);
        }
    }
}