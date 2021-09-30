using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutomationRunner.Core.Common
{
    public static class JsonSerializer
    {
        public static string SerializeObject(object? value)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        public static T DeserializeObject<T>(string json)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }

        public static dynamic DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }
}
