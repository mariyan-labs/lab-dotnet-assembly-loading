using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Host.Console.Framework48.Formatting;

public static class JsonFormatting
{
    extension<T>(T value)
    {
        public string Json()
        {
            return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver =  new CamelCasePropertyNamesContractResolver(),
                Converters = [new StringEnumConverter(new CamelCaseNamingStrategy())]
            });
        }
    }
}