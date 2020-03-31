using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace typingDNAInterface.Models
{
    public partial class VerifyUser
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("result")]
        public int Result { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("confidence_interval")]
        public int ConfidenceInterval { get; set; }

        [JsonProperty("confidence")]
        public int Confidence { get; set; }

        [JsonProperty("net_score")]
        public int NetScore { get; set; }

        [JsonProperty("device_similarity")]
        public int DeviceSimilarity { get; set; }

        [JsonProperty("positions")]
        public int[] Positions { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }



    }

    public partial class VerifyUser
    {
        public static VerifyUser FromJson(string json)
        {
            return JsonConvert.DeserializeObject<VerifyUser>(json, VerifyUserConverter.Settings);
        }
    }

    internal static class VerifyUserConverter
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
