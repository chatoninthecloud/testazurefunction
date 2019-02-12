using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using AzureFunction.Dtos;

namespace AzureFunction.Services
{
    public class AuthenticationService
    {
        private readonly AzureCredentials _azureCredentials;
        public AuthenticationService(string clientId, string clientSecret, string tenantId)
        {
            _azureCredentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);       
        }

        public IAzure Authenticate(string subscriptionId)
        {
            var azure = Azure
            .Configure()
            .Authenticate(_azureCredentials)
            .WithSubscription(subscriptionId);

            if(azure == null)
            {
                throw new AuthenticationException($"Can't connect to subscription {subscriptionId} with service principal {_azureCredentials.ClientId}");
            }

            return azure;            
        }

        public IResourceManager GetResourceManager(string subscriptionId)
        {
            var resourceManager = ResourceManager                    
                .Configure()
                .Authenticate(_azureCredentials)
                .WithSubscription(subscriptionId);

            if(resourceManager == null)
            {
                throw new AuthenticationException($"Can't connect to subscription {subscriptionId} with service principal {_azureCredentials.ClientId}");
            }

            return resourceManager;            
        }
    }
}