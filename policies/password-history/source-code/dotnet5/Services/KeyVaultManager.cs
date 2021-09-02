using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace password_history.Services
{

    public class KeyVaultManager
    {
        private readonly SecretClient _kvClient;
        private readonly IConfiguration _config;

        public KeyVaultManager(IConfiguration config)
        {
            _config = config;

            var cred = new DefaultAzureCredential();
            _kvClient = new SecretClient(new Uri(config["KeyVaultUri"]), cred);
        }

        public async Task<bool> CheckHash(string username, string hash, int prevPwdCount)
        {
            var versionProps = _kvClient.GetPropertiesOfSecretVersions(username);
            var versionPages = versionProps.AsPages().First();
            var count = 0;

            var latest5Versions = versionPages.Values.OrderByDescending(k => k.CreatedOn).Take(Convert.ToInt32(prevPwdCount));

            foreach (var ver in latest5Versions)
            {
                var secret = await _kvClient.GetSecretAsync(username, ver.Version);
                if (secret.Value.Value.Equals(hash))
                {
                    return false;
                }
                count++;

                if (count == 5)
                {
                    return true;
                }
            }

            return true;
        }

        public async Task<bool> SetSecret(string username, string hash)
        {
            await _kvClient.SetSecretAsync(username, hash);
            return true;
        }
    }
}