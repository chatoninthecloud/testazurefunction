namespace AzureFunction.Services
{
    public interface IConfigurationService
    {        
        string ReferentialStorageConnectionString{get;}
        string ReferentialContainerName{get;}
        string ReferentialBlobName{get;}
        string KeyVaultName{get;}
        string LogStorageConnectionString{get;}
        string LogContainerName{get;}
        string LogTableName{get;}
    }
}