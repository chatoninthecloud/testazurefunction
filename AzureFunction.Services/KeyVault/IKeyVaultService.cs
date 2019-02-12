using System.Threading.Tasks;

namespace AzureFunction.Services
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretValueAsync(string secretName);
    }
}