using AutomationRunner.Core.Config;
using AutomationRunner.Core.Secrets;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AutomationRunner.Core.Common
{
    public class HomeAssistantHttpClientFactory
    {
        private readonly HomeAssistantConfiguration homeAssistantConfiguration;
        private readonly SecretsConfig secretsConfig;

        public HomeAssistantHttpClientFactory(HomeAssistantConfiguration homeAssistantConfiguration,
            SecretsConfig secretsConfig)
        {
            this.homeAssistantConfiguration = homeAssistantConfiguration;
            this.secretsConfig = secretsConfig;
        }

        public HttpClient GetHomeAssistantHttpClient()
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

            return client;
        }
    }
}
