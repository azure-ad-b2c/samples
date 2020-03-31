using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace typingDNAInterface.Models
{
    public partial class SaveUser
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public partial class SaveUser
    {
        public static SaveUser FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SaveUser>(json, SaveUserConverter.Settings);
        }
    }

    internal static class SaveUserConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
