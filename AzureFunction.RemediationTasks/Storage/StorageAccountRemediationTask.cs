using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Fluent;
using AzureFunction.Dtos;

namespace AzureFunction.RemediationTasks
{
    public class StorageAccountRemediationTask : RemediationTask
    {
        public StorageAccountRemediationTask(string queueItem, ILogger log): base(queueItem,log, "Microsoft.Storage", "Microsoft.Storage/storageAccounts/write")
        {            
        }

        public override async Task<(bool ResourceUpdated, string LogMessage)> ProcessRemediationAsync()
        {
            var storage = await Azure.StorageAccounts.GetByIdAsync(MessageData.ResourceUri);
            var hasStorageToBeUpdated = false;
            var remediationLogMessage = new StringBuilder();
            if(storage.IsAccessAllowedFromAllNetworks)
            {
                hasStorageToBeUpdated = true;
                remediationLogMessage.AppendLine("Storage Account Vnet Endpoint activated.");
            }
            var allowedIpRules = new List<string>
            {
                "82.255.40.0/22",
                "82.255.56.0/22"
            };

            var missingIpRules = allowedIpRules.Except(storage.IPAddressRangesWithAccess).ToList();
            var unAllowedIpRules = storage.IPAddressRangesWithAccess.Except(allowedIpRules).ToList();

            if(missingIpRules.Any())
            {
                hasStorageToBeUpdated = true;
                remediationLogMessage.AppendLine($"Added Storage Account Vnet Endpoint missing allowed IP Rules {string.Join(',',missingIpRules)}.");
            }

            if(unAllowedIpRules.Any())
            {
                hasStorageToBeUpdated = true;
                remediationLogMessage.AppendLine($"Removed Storage Account Vnet Endpoint unallowed IP Rules {string.Join(',',unAllowedIpRules)}.");
            }

            if(hasStorageToBeUpdated)
            {
                var update = storage.Update().WithAccessFromSelectedNetworks();
                missingIpRules.ForEach(ip => update = update.WithAccessFromIpAddressRange(ip));
                unAllowedIpRules.ForEach(ip => update = update.WithoutIpAddressRangeAccess(ip));
                await update.ApplyAsync();
            }
            else
            {
                remediationLogMessage.AppendLine("Storage is compliant. No update required.");
            }

            Logger.LogInformation(remediationLogMessage.ToString());
            return (hasStorageToBeUpdated, remediationLogMessage.ToString());
        }
    }
}