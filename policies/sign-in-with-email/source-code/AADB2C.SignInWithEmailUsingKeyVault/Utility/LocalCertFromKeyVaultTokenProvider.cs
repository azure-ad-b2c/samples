using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class LocalCertFromKeyVaultTokenProvider : ITokenProvider
    {
        private Lazy<X509SigningCredentials> _signingCredentials;

        public LocalCertFromKeyVaultTokenProvider(Uri vaultUri, string certificateName, TokenCredential azureTokenCredential)
        {
            _signingCredentials = new Lazy<X509SigningCredentials>(() =>
            {
                var keyVaultClient = new Azure.Security.KeyVault.Certificates.CertificateClient(vaultUri, azureTokenCredential);
                var downloadCertificateResult = keyVaultClient.DownloadCertificate(certificateName);   // Use download instead of "Get" to retrieve the full certificate with Private Key.
                                                                                                       // Alternatively, could code differently to ask KV to do the signing. Note that the KV-signs approach could result in throttling depending on load.
                var cert = downloadCertificateResult.Value;
                var result = new X509SigningCredentials(cert);
                return result;
            });
        }

        public string BuildSerializedIdToken(string issuer, string audience, int duration, string userEmail)
        {
            // Parameters that are transmited in the ID Token assertion are communicated as claims
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("email", userEmail, System.Security.Claims.ClaimValueTypes.String, issuer)
            };

            // Create a signed token
            var token = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(duration),
                    _signingCredentials.Value);

            // Serialize the signed token
            var jwtHandler = new JwtSecurityTokenHandler();
            var result = jwtHandler.WriteToken(token);

            return result;
        }
    }
}
