using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AADB2C.MagicLink.WebApi.Models
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

        public static JwksKeyModel FromSigningCredentials(X509SigningCredentials signingCredentials)
        {
            X509Certificate2 certificate = signingCredentials.Certificate;

            // JWK cert data must be base64 (not base64url) encoded
            string certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            // JWK thumbprints must be base64url encoded (no padding or special chars)
            string thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            RSACng rsa = certificate.PublicKey.Key as RSACng; ;

            if (rsa == null)
            {
                throw new Exception("Certificate is not an RSA certificate.");
            }

            RSAParameters keyParams = rsa.ExportParameters(false);
            string keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            string keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);

            return new JwksKeyModel
            {
                Kid = signingCredentials.Kid,
                Kty = "RSA",
                Nbf = new DateTimeOffset(certificate.NotBefore).ToUnixTimeSeconds(),
                Use = "sig",
                Alg = signingCredentials.Algorithm,
                X5C = new[] { certData },
                X5T = thumbprint,
                N = keyModulus,
                E = keyExponent
            };
        }
    }
}
