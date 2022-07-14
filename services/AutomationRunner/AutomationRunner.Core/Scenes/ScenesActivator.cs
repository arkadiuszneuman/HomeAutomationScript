using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Scenes.Specific;
using Microsoft.Extensions.Logging;
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

        private readonly Dictionary<string, TaskWithCancellationToken> activatedScenes = new Dictionary<string, TaskWithCancellationToken>();

        private class TaskWithCancellationToken
        {
            public Task Task { get; set; }
            public CancellationTokenSource CancellationTokenSource { get; set; }

            public TaskWithCancellationToken(Task task, CancellationTokenSource cancellationTokenSource)
            {
                Task = task;
                CancellationTokenSource = cancellationTokenSource;
            }
        }

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
            logger.LogInformation("Starting scene activation listeners {Listeners}", scenes.Select(a => a.Name));

            webSocketConnector.SubscribeActivateScene(OnSceneActivated);
            await webSocketConnector.Start(cancellationToken);
        }

        private async Task OnSceneActivated(string sceneName)
        {
            var sceneToActivate = GetSceneToActivate(sceneName);

            if (sceneToActivate != null)
            {
                if (activatedScenes.ContainsKey(sceneName))
                {
                    var activatedSceneTask = activatedScenes[sceneName];
                    if (!activatedSceneTask.Task.IsCompleted)
                        activatedSceneTask.CancellationTokenSource.Cancel();

                    activatedScenes.Remove(sceneName);
                }
                
                await connector.RefreshStates();

                var cancellationTokenSource = new CancellationTokenSource();
                var sceneTask = sceneToActivate.Activated(cancellationTokenSource.Token);

                activatedScenes.Add(sceneName, new TaskWithCancellationToken(sceneTask, cancellationTokenSource));

                logger.LogInformation("Activated scene {sceneName}", sceneToActivate.Name);
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
