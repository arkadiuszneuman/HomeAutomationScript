using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutomationRunner.Common
{
    public static class JsonSerializer
    {
        public static string SerializeObject(object? value)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        public static T DeserializeObject<T>(string json)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }
    }
}
