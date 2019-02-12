using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using AzureFunction.RemediationTasks;
using Microsoft.Extensions.Logging;

namespace azurefunctions
{
    public static class StorageRemediation
    {
        // [FunctionName("StorageRemediation")]
        // public static async Task RunStorageRemediationAsync([QueueTrigger("storagequeue", Connection = "AzureWebJobsStorage")]string queueItem, ILogger log)
        // {
        //     var storageRemediation = new StorageAccountRemediationTask(queueItem,log);
        //     await storageRemediation.RunAsync();
        // }

        [FunctionName("StorageContainerRemediation")]
        public static async Task RunStorageContainerRemediationAsync([QueueTrigger("storagequeue", Connection = "AzureWebJobsStorage")]string queueItem, ILogger log)
        {
            var storageContainerRemediation = new StorageContainerRemediationTask(queueItem,log);
            await storageContainerRemediation.RunAsync();
        }
    }
}
