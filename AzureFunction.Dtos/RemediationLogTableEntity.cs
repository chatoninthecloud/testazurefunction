using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunction.Dtos
{
    public class RemediationLogTableEntity : TableEntity
    {
        public RemediationLogTableEntity()
        {
            
        }
        public RemediationLogTableEntity(EventGridEventData messageData)
        {
            PartitionKey = messageData.CorrelationId;
            RowKey = messageData.ResourceProvider;
            TenantId = messageData.TenantId;
            SubscriptionId = messageData.SubscriptionId;            
            ResourceProvider = messageData.ResourceProvider;
            ResourceUri =messageData.ResourceUri;
            CorrelationId = messageData.CorrelationId;
        }

        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string ResourceProvider { get; set; }
        public string ResourceUri { get; set; }
        public string CorrelationId { get; set; }
        public string RemediationDetails { get; set; }
        public string EventBlobUri { get; set; }

    }
}