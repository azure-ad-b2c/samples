using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureADB2C.Invite.Models
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
