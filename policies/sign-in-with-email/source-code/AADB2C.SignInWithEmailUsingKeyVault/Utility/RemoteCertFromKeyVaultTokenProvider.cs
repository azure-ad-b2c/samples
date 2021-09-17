using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Certificates;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class RemoteCertFromKeyVaultTokenProvider : ITokenProvider
    {
        private readonly Lazy<VaultCryptoValues> _vaultCryptoValues;

        public RemoteCertFromKeyVaultTokenProvider(Uri vaultUri, string certificateName, TokenCredential azureTokenCredential)
        {
            _vaultCryptoValues = new Lazy<VaultCryptoValues>(() =>
            {
                var vaultCertificateClient = new CertificateClient(vaultUri, azureTokenCredential);
                var getCertificateResult = vaultCertificateClient.GetCertificate(certificateName);

                var keyId = getCertificateResult.Value.KeyId;

                var cert = new X509Certificate2(getCertificateResult.Value.Cer);
                var signingCreds = new X509SigningCredentials(cert);

                var result = new VaultCryptoValues(keyId, signingCreds, azureTokenCredential);
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
            var header = new JwtHeader(_vaultCryptoValues.Value.SigningCredentials);
            var payload = new JwtPayload(
                    issuer,
                    audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(duration));

            // Use the intended JWT Token's Header and Payload value as the data for the token's Signature
            var unsignedTokenText = $"{header.Base64UrlEncode()}.{payload.Base64UrlEncode()}";
            var byteData = Encoding.UTF8.GetBytes(unsignedTokenText);

            // Hash the data to be signed
            var hasher = new SHA256CryptoServiceProvider();
            var digest = hasher.ComputeHash(byteData);

            // Use KV to sign the hashed data
            var vaultCryptoClient = new CryptographyClient(_vaultCryptoValues.Value.CertificateKeyId, _vaultCryptoValues.Value.AzureTokenCredential);
            var signatureResult = vaultCryptoClient.Sign(_vaultCryptoValues.Value.SigningCredentials.Algorithm, digest);
            var encodedSignature = Base64UrlEncoder.Encode(signatureResult.Signature);

            // Assemble the header, payload, and encoded signatures
            var result = $"{unsignedTokenText}.{encodedSignature}";
            return result;
        }

        private class VaultCryptoValues
        {
            public VaultCryptoValues(Uri certificateKeyId, SigningCredentials signingCredentials, TokenCredential azureTokenCredential)
            {
                CertificateKeyId = certificateKeyId ?? throw new ArgumentNullException(nameof(certificateKeyId));
                SigningCredentials = signingCredentials ?? throw new ArgumentNullException(nameof(signingCredentials));
                AzureTokenCredential = azureTokenCredential ?? throw new ArgumentNullException(nameof(azureTokenCredential));
            }

            public Uri CertificateKeyId { get; }
            public SigningCredentials SigningCredentials { get; }
            public TokenCredential AzureTokenCredential { get; }
        }
    }
}
