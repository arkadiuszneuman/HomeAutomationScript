using AutomationRunner.Core.Common.Connector.WebSocketConnector;
using AutomationRunner.Core.Secrets;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Common.Connector
{
    public class HomeAssistantWebSocketConnector
    {
        private readonly IWebSocketConnector webSocketConnector;
        private readonly SecretsConfig secretsConfig;

        public HomeAssistantWebSocketConnector(IWebSocketConnector webSocketConnector,
            SecretsConfig secretsConfig)
        {
            this.webSocketConnector = webSocketConnector;
            this.secretsConfig = secretsConfig;
        }

        public async Task Init(CancellationToken cancellationToken = default)
        {
            await webSocketConnector.OnResponse(OnResponse, cancellationToken);
            await SendAuth(cancellationToken);
            await SendSubscribeEvents(cancellationToken);
        }

        private async Task SendSubscribeEvents(CancellationToken cancellationToken)
        {
            await Send(new
            {
                id = 1,
                type = "subscribe_events"
            }, cancellationToken);
        }

        private void OnResponse(string response)
        {

        }

        private async Task SendAuth(CancellationToken cancellationToken)
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
    }
}
