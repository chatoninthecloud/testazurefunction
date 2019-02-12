using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace AzureFunction.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly string _keyVaultName;
        private readonly IKeyVaultClient _keyVaultClient;
        public KeyVaultService(string keyVaultName)
        {
            _keyVaultName = keyVaultName;
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            _keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }
        public async Task<string> GetSecretValueAsync(string secretName)
        {
            var secret = await _keyVaultClient.GetSecretAsync($"https://{_keyVaultName}.vault.azure.net/secrets/{secretName}"); 
            return secret.Value;           
        }
    }
}