using AutomationRunner.Common;
using AutomationRunner.Config;
using AutomationRunner.Secrets;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner
{
    public class DaemonService : IHostedService
    {
        private readonly HomeAssistantHttpClientFactory httpClientFactory;

        public DaemonService(HomeAssistantHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var client = httpClientFactory.GetHomeAssistantHttpClient();
            var xxx = await (await client.GetAsync("api/events")).Content.ReadAsStringAsync();

            Console.WriteLine(xxx);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
