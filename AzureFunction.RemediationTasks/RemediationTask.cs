using System;
using System.Threading.Tasks;
using AzureFunction.Dtos;
using AzureFunction.Services;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunction.RemediationTasks
{
    public abstract class RemediationTask
    {
        private readonly string _queueItem;
        private readonly string _operationName;
        private readonly string _resourceProvider;
        private  string _resourceGroupName;
        private  string _resourceName;
        private IResourceManager _resourceManager;
        public ILogger Logger{get;}
        public ReferentialEntity ReferentialEntity {get; private set;}
        public EventGridEvent Message{get; private set;}
        public EventGridEventData MessageData{get; private set;}
        public IAzure Azure{get; private set;}
        public IConfigurationService ConfigurationService{get; private set;}        
        public RemediationTask(string queueItem, ILogger log, string resourceProvider, string operationName)
        {
            _queueItem = queueItem;
            Logger = log;
            _operationName = operationName;
            _resourceProvider = resourceProvider;
        }

        private async Task ConfigureAsync()
        {
            // Get information from queue item 
            Message = JsonConvert.DeserializeObject<EventGridEvent>(_queueItem);
            MessageData = JsonConvert.DeserializeObject<EventGridEventData>(Message.Data.ToString());
            Logger.LogInformation(MessageData.ToString());

            // Ensure Operation match remediation action
            MessageData.EnsureMessageActionAndResourceProvider(_resourceProvider, _operationName);

            // Get Referential entry for this subscription
            ConfigurationService = new ConfigurationService();
            IReferentialService referentialService = new ReferentialService(ConfigurationService);                
            ReferentialEntity = await referentialService.GetReferentialEntityAsync(MessageData.SubscriptionId);

            // Get Service Principal key from KeyVault for this subscription
            IKeyVaultService keyVaultService = new KeyVaultService(ConfigurationService.KeyVaultName);
            var servicePrincipalKey = await keyVaultService.GetSecretValueAsync(ReferentialEntity.ServicePrincipalId);

            // Connect to Azure
            var authenticationService = new AuthenticationService(ReferentialEntity.ServicePrincipalId, servicePrincipalKey, MessageData.TenantId);
            Azure = authenticationService.Authenticate(MessageData.SubscriptionId);
            _resourceManager = authenticationService.GetResourceManager(MessageData.SubscriptionId);           
        }

        public async Task RunAsync()
        {
            await ConfigureAsync();
            (_resourceName, _resourceGroupName) = await GetResourceAndResourceGroupName(); 
            if(_resourceGroupName == ReferentialEntity.OpenResourceGroup)
            {
                Logger.LogInformation("Event for resource located in Open Resource Group. No remediation.");
                return;
            }
            var remediationExecutionResult = await ProcessRemediationAsync();
            await LogResultAsync(remediationExecutionResult);
        }

        private async Task LogResultAsync((bool ResourceUpdated, string LogMessage) remediationExecutionResult)
        {
            IRemediationLogService logService = new RemediationLogService(ConfigurationService);
            await logService.LogRemediationExecution(remediationExecutionResult.ResourceUpdated, remediationExecutionResult.LogMessage, MessageData, Message, _queueItem, _resourceGroupName);
        }

        public virtual async Task<(string resourceName, string resourceGroupName)> GetResourceAndResourceGroupName()
        {
            var resource = await _resourceManager.GenericResources.GetByIdAsync(MessageData.ResourceUri);
            return (resource.Name, resource.ResourceGroupName);
        }

        public abstract Task<(bool ResourceUpdated, string LogMessage)> ProcessRemediationAsync();
    }

    
}