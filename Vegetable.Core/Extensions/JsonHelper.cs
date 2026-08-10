using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Vegetable.Core.Extensions
{
    public static class JsonHelper
    {
        public static T ToObject<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new CamelCaseOnlyConverter() }
            };

            var typedObject = JsonConvert.DeserializeObject<T>(json, settings);
            return typedObject;
        }
    }

}
