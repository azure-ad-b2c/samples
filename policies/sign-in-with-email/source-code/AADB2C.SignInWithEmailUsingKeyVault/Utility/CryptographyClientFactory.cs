using System;
using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public class CryptographyClientFactory
    {
        private readonly TokenCredential _azureTokenCredential;

        public CryptographyClientFactory(TokenCredential azureTokenCredential)
        {
            _azureTokenCredential = azureTokenCredential ?? throw new ArgumentNullException(nameof(azureTokenCredential));
        }

        public CryptographyClient CreateCryptographyClient(Uri keyId)
        {
            return new CryptographyClient(keyId, _azureTokenCredential);
        }
    }
}
