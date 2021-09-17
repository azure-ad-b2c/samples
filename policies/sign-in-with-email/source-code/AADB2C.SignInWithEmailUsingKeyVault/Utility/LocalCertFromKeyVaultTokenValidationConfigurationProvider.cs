using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using AADB2C.SignInWithEmailUsingKeyVault.Models;
using Azure.Core;
using Newtonsoft.Json;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class LocalCertFromKeyVaultTokenValidationConfigurationProvider : ITokenValidationConfigurationProvider
    {
        private Lazy<X509SigningCredentials> _signingCredentials;

        public LocalCertFromKeyVaultTokenValidationConfigurationProvider(Uri vaultUri, string certificateName, TokenCredential azureTokenCredential) 
        {
            _signingCredentials = new Lazy<X509SigningCredentials>(() =>
            {
                var keyVaultClient = new Azure.Security.KeyVault.Certificates.CertificateClient(vaultUri, azureTokenCredential);
                var getCertificateResult = keyVaultClient.GetCertificate(certificateName);  // Use GetCertificate instead of Download b/c the Private key is not needed (nor wanted!) for building validation metadata
                var cert = new X509Certificate2(getCertificateResult.Value.Cer);
                var result = new X509SigningCredentials(cert);
                return result;
            });
        }

        public string BuildSerializedOidcConfig(string issuer, string jwksUri)
        {
            var result = new OidcModel
            {
                Issuer = issuer,
                JwksUri = jwksUri,
                IdTokenSigningAlgValuesSupported = new[] { _signingCredentials.Value.Algorithm },
            };
            var serializedResult = JsonConvert.SerializeObject(result);
            return serializedResult;
        }


        public string BuildSerializedJwks()
        {
            var certificate = _signingCredentials.Value.Certificate;

            // JWK cert data must be base64 (not base64url) encoded
            string certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            // JWK thumbprints must be base64url encoded (no padding or special chars)
            string thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            var rsa = certificate.PublicKey.Key as RSACng;

            if (rsa == null)
            {
                throw new InvalidOperationException("Certificate is not an RSA certificate.");
            }

            var keyParams = rsa.ExportParameters(false);
            var keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            var keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);

            var keyModel = new JwksKeyModel
            {
                Kid = _signingCredentials.Value.Kid,
                Kty = "RSA",
                Nbf = new DateTimeOffset(certificate.NotBefore).ToUnixTimeSeconds(),
                Use = "sig",
                Alg = _signingCredentials.Value.Algorithm,
                X5C = new[] { certData },
                X5T = thumbprint,
                N = keyModulus,
                E = keyExponent
            };

            var result = new JwksModel
            {
                Keys = new[] { keyModel }
            };
            var serializedResult = JsonConvert.SerializeObject(result);
            return serializedResult;
        }
    }
}
