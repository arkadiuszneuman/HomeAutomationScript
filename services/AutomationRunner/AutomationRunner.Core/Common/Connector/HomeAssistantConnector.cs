﻿using AutomationRunner.Core.Entities;
using AutomationRunner.Core.Entities.Services.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Common.Connector
{
    public class HomeAssistantConnector
    {
        private readonly ILogger<HomeAssistantConnector> logger;
        private readonly HomeAssistantHttpClientFactory clientFactory;

        private string? loadedStates;

        public HomeAssistantConnector(
            ILogger<HomeAssistantConnector> logger,
            HomeAssistantHttpClientFactory clientFactory)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
        }

        public async Task<string> LoadFromEntityId(string entityId)
        {
            using var client = clientFactory.GetHomeAssistantHttpClient();
            var result = await (await client.GetAsync($"api/states/{entityId}")).Content.ReadAsStringAsync();
            if (result.Contains("Entity not found"))
                throw new EntityNotFoundException(entityId);

            return result;
        }

        public async Task<T> LoadEntityFromStates<T>(string entityId)
            where T : BaseEntity
        {
            var entities = await LoadAll<T>();
            var selectedEntity = entities.SingleOrDefault(e => e.EntityId == entityId);
            if (selectedEntity == null)
                throw new EntityNotFoundException(entityId);

            selectedEntity.Connector = this;

            return selectedEntity;
        }

        public async Task<IEnumerable<T>> LoadAll<T>()
            where T : BaseEntity
        {
            if (string.IsNullOrEmpty(loadedStates))
                await RefreshStates();

            return JsonSerializer.DeserializeObject<IEnumerable<T>>(loadedStates);
        }

        public async Task SendServiceAsync<T>(string serviceId, T service, CancellationToken cancellationToken = default)
            where T : IService
        {
            using var client = clientFactory.GetHomeAssistantHttpClient();
            var uri = $"api/services/{serviceId.Replace('.', '/')}";
            var json = JsonSerializer.SerializeObject(service);
            var response = await client.PostAsync(uri,
                new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
                logger.LogError("Error while sending status to {0} with body {1}", uri, json);

            await Task.Delay(5, cancellationToken); // bug in HA, we need to wait between each activations
        }

        public async Task RefreshStates()
        {
            using var client = clientFactory.GetHomeAssistantHttpClient();
            loadedStates = await (await client.GetAsync($"api/states")).Content.ReadAsStringAsync();
        }
    }
}
