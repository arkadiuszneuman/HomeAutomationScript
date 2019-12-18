using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Common.Connector.WebSocketConnector
{
    public interface IWebSocketConnector
    {
        Task Connect(CancellationToken cancellationToken = default);
        Task OnResponse(Func<string, Task> response, CancellationToken cancellationToken = default);
        Task SendAsync(string message, CancellationToken cancellationToken = default);
    }
}