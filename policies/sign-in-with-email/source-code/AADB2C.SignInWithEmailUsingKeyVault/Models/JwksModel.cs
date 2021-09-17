using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AADB2C.SignInWithEmailUsingKeyVault.Models
{
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
