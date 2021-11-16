using System.Collections.Generic;
using Newtonsoft.Json;

namespace AADMagicLinks.Models
{
    public class OidcModel
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public ICollection<string> IdTokenSigningAlgValuesSupported { get; set; }
    }
}
