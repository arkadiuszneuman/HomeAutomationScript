using AutomationRunner.Core.Common.Connector.WebSocketConnector;
using AutomationRunner.Core.Secrets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AutomationRunner.Core.Common.Connector
{
    public partial class HomeAssistantWebSocketConnector
    {
        private readonly ILogger<HomeAssistantWebSocketConnector> logger;
        private readonly IWebSocketConnector webSocketConnector;
        private readonly SecretsConfig secretsConfig;
        private readonly HomeAssistantConnector connector;

        private readonly Dictionary<Guid, Func<string, Task>> subscribedSceneActivations =
            new Dictionary<Guid, Func<string, Task>>();

        public HomeAssistantWebSocketConnector(
            ILogger<HomeAssistantWebSocketConnector> logger,
            IWebSocketConnector webSocketConnector,
            SecretsConfig secretsConfig,
            HomeAssistantConnector connector)
        {
            this.logger = logger;
            this.webSocketConnector = webSocketConnector;
            this.secretsConfig = secretsConfig;
            this.connector = connector;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            await webSocketConnector.OnResponse(OnResponse, cancellationToken);
        }

        private async Task SendSubscribeEvents(CancellationToken cancellationToken = default)
        {
            await Send(new
            {
                id = 1,
                type = "subscribe_events"
            }, cancellationToken);
        }


        private async Task SendAuth(CancellationToken cancellationToken = default)
        {
            await Send(new
            {
                type = "auth",
                access_token = secretsConfig.HomeAssistantToken
            }, cancellationToken);
        }

        private async Task Send<T>(T @object, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.SerializeObject(@object);
            await webSocketConnector.SendAsync(json, cancellationToken);
        }

        private async Task OnResponse(string response)
        {
            dynamic responseObject = JsonSerializer.DeserializeObject(response);
            string type = responseObject.type;
            string eventType = responseObject.@event?.event_type;
            string domain = responseObject.@event?.data?.domain;
            string service = responseObject.@event?.data?.service;

            var task = (type, eventType, domain, service) switch
            {
                ("event", "state_changed", _, _) => HandleStateChangedEvent(response),
                ("event", "call_service", "scene", "turn_on") => HandleSceneActivation(response),
                ("auth_required", _, _, _) => SendAuth().ContinueWith(s => SendSubscribeEvents()),
                ("auth_ok", _, _, _) => Task.Factory.StartNew(() => logger.LogInformation("Successfully logged in")),
                ("result", _, _, _) => Task.Factory.StartNew(() => logger.LogDebug("Successfully subscribed for events")),
                _ => Task.Factory.StartNew(() => logger.LogDebug("No data for type {Type}: {Json}", type, response))
            };

            await task;
        }

        private async Task HandleSceneActivation(string response)
        {
            var callServiceObject = JsonSerializer.DeserializeObject<CallServiceResponse>(response);
            foreach (var subscribedSceneActivation in subscribedSceneActivations.Values)
                await subscribedSceneActivation(callServiceObject.Event.Data.ServiceData["entity_id"].ToString());
        }   

        private Task HandleStateChangedEvent(string response)
        {
            var stateChangedObject = JsonSerializer.DeserializeObject<StateChangedResponse>(response);
            stateChangedObject.Event.Data.NewState.Connector
                = stateChangedObject.Event.Data.OldState.Connector
                = connector;

            return Task.CompletedTask;
        }

        public Guid SubscribeActivateScene(Func<string, Task> action)
        {
            var sceneId = Guid.NewGuid();
            subscribedSceneActivations.Add(sceneId, action);
            return sceneId;
        }
    }
}
