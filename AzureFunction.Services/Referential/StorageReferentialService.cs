using Microsoft.WindowsAzure.Storage;
using AzureFunction.Dtos;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace AzureFunction.Services
{
    public class ReferentialService : IReferentialService
    {
        private readonly IConfigurationService _configurationService;
        public ReferentialService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;            
        }

        public async Task<ReferentialEntity> GetReferentialEntityAsync(string subscriptionId)
        {

            var connectionString = _configurationService.ReferentialStorageConnectionString;
            var containerName = _configurationService.ReferentialContainerName;
            var blobName = _configurationService.ReferentialBlobName;

            CloudStorageAccount storageAccount = null;
            if(!(CloudStorageAccount.TryParse(connectionString, out storageAccount)))
            {
                throw new AuthenticationException("Can't access Referential.");
            }
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);  
            if(!(await blobContainer.ExistsAsync()))
            {
                throw new ReferentialException($"There is no container {containerName} in specified storage Account");
            }
            var referentialBlob = blobContainer.GetBlockBlobReference(blobName);

            if(!(await referentialBlob.ExistsAsync()))
            {
                throw new ReferentialException($"Referential file {blobName} does not exist in container {containerName}");
            }

            var blobContent = await referentialBlob.DownloadTextAsync();
            var referential = JsonConvert.DeserializeObject<IList<ReferentialEntity>>(blobContent);
            var subscriptionReferential = referential.FirstOrDefault(r => r.SubscriptionId == subscriptionId);
            if (subscriptionReferential == null)
            {
                throw new ReferentialException($"There is no entry for subscription {subscriptionId} in referential");
            }
            return subscriptionReferential;
        }
    }
}