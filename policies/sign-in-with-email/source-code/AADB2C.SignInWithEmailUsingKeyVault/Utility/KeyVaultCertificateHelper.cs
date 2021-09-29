using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AADB2C.SignInWithEmailUsingKeyVault.Models;
using Azure.Security.KeyVault.Certificates;
using Newtonsoft.Json;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class KeyVaultCertificateHelper
    {
        private readonly CertificateClient _certificateClient;
        private readonly CryptographyClientFactory _cryptographyClientFactory;
        private readonly Lazy<VaultCryptoValues> _vaultCryptoValues;

        public KeyVaultCertificateHelper(CertificateClient certificateClient, CryptographyClientFactory cryptographyClientFactory, IOptions<AppSettingsModel> appSettings)
        {
            _certificateClient = certificateClient ?? throw new ArgumentNullException(nameof(certificateClient));
            _cryptographyClientFactory = cryptographyClientFactory ?? throw new ArgumentNullException(nameof(cryptographyClientFactory));

            // Make the cryptography values read from the Certificate data Lazy<T> so that any vault throttling, retrying,
            // or other delays occur during the Controller calls to this helper, rather than during construction
            var certificateName = appSettings.Value.CertificateName;
            _vaultCryptoValues = new Lazy<VaultCryptoValues>(() => ReadCertificate(certificateName));
        }

        public string BuildSerializedOidcConfig(string issuer, string jwksUri)
        {
            var result = new OidcModel
            {
                Issuer = issuer,
                JwksUri = jwksUri,
                IdTokenSigningAlgValuesSupported = new[] { _vaultCryptoValues.Value.SigningCredentials.Algorithm },
            };
            var serializedResult = JsonConvert.SerializeObject(result);
            return serializedResult;
        }


        public string BuildSerializedJwks()
        {
            var certificate = _vaultCryptoValues.Value.SigningCredentials.Certificate;

            // JWK cert data must be base64 (not base64url) encoded
            string certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            // JWK thumbprints must be base64url encoded (no padding or special chars)
            string thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            RSA rsa = certificate.GetRSAPublicKey();
            if (rsa == null)
            {
                throw new InvalidOperationException("Certificate is not an RSA certificate.");
            }

            var keyParams = rsa.ExportParameters(false);
            var keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            var keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);

            var keyModel = new JwksKeyModel
            {
                Kid = _vaultCryptoValues.Value.SigningCredentials.Kid,
                Kty = "RSA",
                Nbf = new DateTimeOffset(certificate.NotBefore).ToUnixTimeSeconds(),
                Use = "sig",
                Alg = _vaultCryptoValues.Value.SigningCredentials.Algorithm,
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

        public async Task<string> BuildSerializedIdTokenAsync(string issuer, string audience, int duration, string userEmail)
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

            // Use KV to compute the signature
            var cryptographyClient = _cryptographyClientFactory.CreateCryptographyClient(_vaultCryptoValues.Value.CertificateKeyId);
            // SignData will create the digest and encode it (whereas Sign requires that the digest is computed here and to be sent in.)
            var signatureResult = await cryptographyClient
                .SignDataAsync(_vaultCryptoValues.Value.SigningCredentials.Algorithm, byteData)
                .ConfigureAwait(false);
            var encodedSignature = Base64UrlEncoder.Encode(signatureResult.Signature);

            // Alternatively, download the certificate and sign it locally to minimize the request count to the KeyVault instance
            // TODO - Download once - replace ReadCertificate with DownloadCertificate and (CAREFULLY) hold on to the downloaded Private Key
            ////var downloadCertificateResult = _certificateClient.DownloadCertificate(certificateName);   // Use download instead of "Get" to retrieve the full certificate with Private Key.
            ////var downloadedCert = downloadCertificateResult.Value;
            // TODO - Create a local crypto client instance using the RSA Private Key and use it to sign the data
            ////var localCryptographyClient = new Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient(new Azure.Security.KeyVault.Keys.JsonWebKey(downloadedCert.GetRSAPrivateKey()));
            ////var signatureResult = await localCryptographyClient
            ////    .SignDataAsync(_vaultCryptoValues.Value.SigningCredentials.Algorithm, byteData)
            ////    .ConfigureAwait(false);

            // Assemble the header, payload, and encoded signatures
            var result = $"{unsignedTokenText}.{encodedSignature}";
            return result;
        }

        private VaultCryptoValues ReadCertificate(string certificateName)
        {
            var getCertificateResult = _certificateClient.GetCertificate(certificateName);

            var keyId = getCertificateResult.Value.KeyId;

            var cert = new X509Certificate2(getCertificateResult.Value.Cer);
            var signingCreds = new X509SigningCredentials(cert);

            var result = new VaultCryptoValues(keyId, signingCreds);
            return result;
        }

        // Helper class to hold useful values obtained by reading the certificate from KV
        private class VaultCryptoValues
        {
            public VaultCryptoValues(Uri certificateKeyId, X509SigningCredentials signingCredentials)
            {
                CertificateKeyId = certificateKeyId ?? throw new ArgumentNullException(nameof(certificateKeyId));
                SigningCredentials = signingCredentials ?? throw new ArgumentNullException(nameof(signingCredentials));
            }

            public Uri CertificateKeyId { get; }
            public X509SigningCredentials SigningCredentials { get; }
        }
    }
}
