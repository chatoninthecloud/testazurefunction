using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Fluent;
using AzureFunction.Dtos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;

namespace AzureFunction.RemediationTasks
{
    public class StorageContainerRemediationTask : RemediationTask
    {
        public StorageContainerRemediationTask(string queueItem, ILogger log): 
            base(queueItem,log, "Microsoft.Storage", "Microsoft.Storage/storageAccounts/blobServices/containers/write")
        {            
        }

        public override async Task<(string resourceName, string resourceGroupName)> GetResourceAndResourceGroupName()
        {
             var storageAccountContainerRegex = new Regex(@"^\/subscriptions\/\S+\/resourceGroups\/(\S+)\/providers\/Microsoft.Storage\/storageAccounts\/\S+\/blobServices\/default\/containers\/(\S+)");
             var regexMatch = storageAccountContainerRegex.Match(MessageData.ResourceUri);
             return (regexMatch.Groups[2].Value, regexMatch.Groups[1].Value);
        }

        public override async Task<(bool ResourceUpdated, string LogMessage)> ProcessRemediationAsync()
        {
            var storageAccountContainerRegex = new Regex(@"^(\S+)\/blobServices\/default\/containers\/(\S+)");
            var regexMatch = storageAccountContainerRegex.Match(MessageData.ResourceUri);
            var storageUri = regexMatch.Groups[1].Value;
            var containerName = regexMatch.Groups[2].Value;
            var storage = await Azure.StorageAccounts.GetByIdAsync(storageUri);
            var storageKeys = await storage.GetKeysAsync();
            var primaryKey = storageKeys.FirstOrDefault();
            var storageConnectionString = $"DefaultEndpointsProtocol=http;AccountName={storage.Name};AccountKey={primaryKey.Value}";   
            CloudStorageAccount storageAccount = null;
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

            var hasContainerToBeUpdated = false;
            var remediationLogMessage = new StringBuilder();
            if(container.Properties.PublicAccess != BlobContainerPublicAccessType.Off)
            {
                hasContainerToBeUpdated = true;
                remediationLogMessage.AppendLine($"Container Public Acces Type set to Off (was {container.Properties.PublicAccess.ToString()}");
            }

            if(hasContainerToBeUpdated)
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions{PublicAccess = BlobContainerPublicAccessType.Off});
            }
            
            return (hasContainerToBeUpdated, remediationLogMessage.ToString());
        }
    }
}