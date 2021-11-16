using System.Collections.Generic;
using Newtonsoft.Json;

namespace AADMagicLinks.Models
{
    /// <summary>
    /// Representation of the returned claims from AAD
    /// </summary>
    public class JwksModel
    {
        [JsonProperty("keys")]
        public ICollection<JwksKeyModel> Keys { get; set; }
    }
    public class JwksKeyModel
    {
        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("nbf")]
        public long Nbf { get; set; }

        [JsonProperty("use")]
        public string Use { get; set; }

        [JsonProperty("kty")]
        public string Kty { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("x5c")]
        public ICollection<string> X5C { get; set; }

        [JsonProperty("x5t")]
        public string X5T { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }

        [JsonProperty("e")]
        public string E { get; set; }
    }
}
