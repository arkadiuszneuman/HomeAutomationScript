﻿using AutomationRunner.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Common.Connector
{
    public class HomeAssistantConnector
    {
        private readonly HomeAssistantHttpClientFactory clientFactory;
        private string loadedStates;

        public HomeAssistantConnector(HomeAssistantHttpClientFactory clientFactory)
        {
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

            return selectedEntity;
        }

        public async Task<IEnumerable<T>> LoadAll<T>()
            where T : BaseEntity
        {
            if (string.IsNullOrEmpty(loadedStates))
                await RefreshStates();

            return JsonConvert.DeserializeObject<IEnumerable<T>>(loadedStates);
        }

        public async Task SendService(string serviceId, EntityIdService entityIdService)
        {
            using var client = clientFactory.GetHomeAssistantHttpClient();
            await client.PostAsync($"api/services/{serviceId.Replace('.', '/')}",
                new StringContent(JsonConvert.SerializeObject(entityIdService), Encoding.UTF8, "application/json"));
        }

        public async Task RefreshStates()
        {
            using var client = clientFactory.GetHomeAssistantHttpClient();
            loadedStates = await (await client.GetAsync($"api/states")).Content.ReadAsStringAsync();
        }
    }
}