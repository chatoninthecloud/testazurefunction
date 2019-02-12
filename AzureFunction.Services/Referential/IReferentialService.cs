using System.Threading.Tasks;
using AzureFunction.Dtos;

namespace AzureFunction.Services
{
    public interface IReferentialService
    {
        Task<ReferentialEntity> GetReferentialEntityAsync(string subscriptionId);
    }
}