using AutomationRunner.Core.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Common.Connector.WebSocketConnector
{
    public class HomeAssistantWebSocket : IWebSocketConnector, IDisposable
    {
        private readonly ILogger<HomeAssistantWebSocket> logger;
        private readonly HomeAssistantConfiguration homeAssistantConfiguration;

        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private ClientWebSocket clientWebSocket;
        private bool disposed;

        public HomeAssistantWebSocket(
            ILogger<HomeAssistantWebSocket> logger,
            HomeAssistantConfiguration homeAssistantConfiguration)
        {
            this.logger = logger;
            this.homeAssistantConfiguration = homeAssistantConfiguration;
        }

        public async Task Connect(CancellationToken cancellationToken = default)
        {
            await semaphoreSlim.WaitAsync();
            int retries = 0;
            try
            {
                while (!IsWebSocketConnected())
                {
                    try
                    {

                        logger.LogInformation("Connecting to HomeAssistant websocket...");

                        if (clientWebSocket != null)
                            clientWebSocket.Dispose();

                        clientWebSocket = new ClientWebSocket();
                        clientWebSocket.Options.RemoteCertificateValidationCallback
                            += (sender, cert, chain, sslPolicyErrors) => true;
                        await clientWebSocket.ConnectAsync(
                            new Uri(homeAssistantConfiguration.WebSocketsUri),
                            cancellationToken);

                        logger.LogInformation("Connected to HomeAssistant websocket...");
                    }
                    catch (WebSocketException)
                    {
                        ++retries;
                        retries = Math.Min(3, retries);
                        var timeForRetry = 3000 * retries;
                        logger.LogWarning("Cannot connect to HomeAssistant. Retrying in {0}...", timeForRetry);
                        await Task.Delay(timeForRetry);
                    }

                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            if (!IsWebSocketConnected())
                await Connect();

            logger.LogDebug("Sending message: {Message}", message);
            await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                WebSocketMessageType.Text, true, cancellationToken);
        }

        public async Task OnResponse(Func<string, Task> response, CancellationToken cancellationToken = default)
        {
            await Task.Factory.StartNew(async () =>
            {
                var bytesReceived = new ArraySegment<byte>(new byte[1000000]);

                while (true)
                {
                    if (!IsWebSocketConnected())
                        await Connect(cancellationToken);

                    var result = await clientWebSocket.ReceiveAsync(bytesReceived, cancellationToken);
                    var message = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                    logger.LogDebug("Received message: {Message}", message);
                    await response(message);
                }
            });
        }

        private bool IsWebSocketConnected()
        {
            return clientWebSocket?.State == WebSocketState.Open;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (clientWebSocket != null)
                    clientWebSocket.Dispose();
            }

            disposed = true;
        }
    }
}
