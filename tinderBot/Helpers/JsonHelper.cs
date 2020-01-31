using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace tinderBot.Helpers
{
    public static class JsonHelper
    {
        public static JsonSerializerSettings GetDefaultJsonSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseContractResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            return serializerSettings;
        }
    }
}
