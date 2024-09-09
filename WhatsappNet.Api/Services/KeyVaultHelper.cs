using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace WhatsappNet.Api.Services
{
    public class KeyVaultHelper
    {
        private readonly SecretClient _client;
        private string keyVaultName = "";
        public KeyVaultHelper(string keyVaultName)
        {
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            _client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            KeyVaultSecret secret = await _client.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}
