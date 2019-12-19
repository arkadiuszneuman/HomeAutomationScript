using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Scenes.Specific;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Scenes
{
    public class ScenesActivator
    {
        private readonly ILogger<ScenesActivator> logger;
        private readonly IEnumerable<IScene> scenes;
        private readonly HomeAssistantWebSocketConnector webSocketConnector;
        private readonly HomeAssistantConnector connector;

        public ScenesActivator(
            ILogger<ScenesActivator> logger,
            IEnumerable<IScene> scenes,
            HomeAssistantWebSocketConnector webSocketConnector,
            HomeAssistantConnector connector)
        {
            this.logger = logger;
            this.scenes = scenes;
            this.webSocketConnector = webSocketConnector;
            this.connector = connector;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Starting scene activation listeners {string.Join(", ", scenes.Select(a => a.Name))}");

            webSocketConnector.SubscribeActivateScene(OnSceneActivated);
            await webSocketConnector.Start(cancellationToken);
        }

        private async Task OnSceneActivated(string sceneName)
        {
            var sceneToActivate = GetSceneToActivate(sceneName);

            if (sceneToActivate != null)
            {
                await connector.RefreshStates();
                sceneToActivate.Activated();
            }
        }

        private IScene GetSceneToActivate(string sceneName)
        {
            var scenesToActivate = scenes.Where(s => s.Name == sceneName).ToList();
            if (scenesToActivate.Count > 1)
                throw new ScenesException($"Too many scenes for scene {sceneName}");

            var sceneToActivate = scenesToActivate.SingleOrDefault();
            return sceneToActivate;
        }
    }
}
