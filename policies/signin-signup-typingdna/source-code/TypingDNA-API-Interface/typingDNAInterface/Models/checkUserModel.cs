using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace typingDNAInterface.Models
{
    public partial class CheckUser
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("mobilecount")]
        public int Mobilecount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
    public partial class CheckUser
    {
        public static CheckUser FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CheckUser>(json, CheckUserConverter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this CheckUser self) => JsonConvert.SerializeObject(self, CheckUserConverter.Settings);
    }

    internal static class CheckUserConverter
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

