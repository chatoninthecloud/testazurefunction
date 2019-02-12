using System.Text;
using Newtonsoft.Json;

namespace AzureFunction.Dtos
{
     public class EventGridEventData
     {

        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty(PropertyName = "resourceProvider")]
        public string ResourceProvider { get; set; }

        [JsonProperty(PropertyName = "resourceUri")]
        public string ResourceUri { get; set; }

        [JsonProperty(PropertyName = "operationName")]
        public string OperationName { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationId { get; set; }

        public override string ToString()
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"Message for Resource {ResourceUri}");
            messageBuilder.Append($" of type {ResourceProvider}");
            messageBuilder.Append($" for operation {OperationName}");
            messageBuilder.Append($" in subscription {SubscriptionId}");
            messageBuilder.Append($" linked to Tenant {TenantId}");
            return messageBuilder.ToString();
        }

        public void EnsureMessageActionAndResourceProvider(string resourceProvider, string operationName)
        {
            if(!(resourceProvider == ResourceProvider && operationName == OperationName))
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.Append($"Resource Provider {ResourceProvider} and Operation {OperationName} do not match");
                messageBuilder.Append($" remediation task exepcted ones ({resourceProvider} / {operationName})");
                throw new EventException(messageBuilder.ToString());
            };
        }
     }
}