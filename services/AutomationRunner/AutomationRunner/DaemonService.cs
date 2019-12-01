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
        private readonly HomeAssistantConfiguration homeAssistantConfiguration;
        private readonly SecretsConfig secretsConfig;

        public DaemonService(HomeAssistantConfiguration homeAssistantConfiguration,
            SecretsConfig secretsConfig)
        {
            this.homeAssistantConfiguration = homeAssistantConfiguration;
            this.secretsConfig = secretsConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(homeAssistantConfiguration.Uri);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretsConfig.HomeAssistantToken}");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            var xxx = client.GetAsync("api/").Result.Content.ReadAsStringAsync().Result;

            Console.WriteLine(xxx);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
