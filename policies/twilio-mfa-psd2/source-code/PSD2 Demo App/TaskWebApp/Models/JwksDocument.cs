using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TaskWebApp.Models
{
    public class JwksDocument
    {
        [JsonProperty("keys")]
        public ICollection<JwksKey> Keys { get; set; }
    }

    public class JwksKey
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

        public static JwksKey FromSigningCredentials(X509SigningCredentials signingCredentials)
        {
            X509Certificate2 certificate = signingCredentials.Certificate;

            // JWK cert data must be base64 (not base64url) encoded
            string certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            // JWK thumbprints must be base64url encoded (no padding or special chars)
            string thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            RSACryptoServiceProvider publicKey = certificate.PublicKey.Key as RSACryptoServiceProvider;
            if (publicKey == null)
            {
                throw new Exception("Certificate is not an RSA certificate.");
            }
            RSAParameters keyParams = publicKey.ExportParameters(false);
            string keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            string keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);
            var certnbf = (certificate.NotBefore).Ticks;

            return new JwksKey
            {
                Kid = signingCredentials.Kid,
                Kty = "RSA",
                Nbf = 1555712377,
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