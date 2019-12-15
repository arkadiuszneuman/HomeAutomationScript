using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Intrinsics.X86;

namespace AutomationRunner.Core.Common
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
            serializerSettings.Error = delegate (object sender, ErrorEventArgs args)
            {
                //errors.Add(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }
    }
}
