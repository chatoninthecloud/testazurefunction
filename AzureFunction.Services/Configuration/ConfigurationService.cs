using System;
using AzureFunction.Dtos;

namespace AzureFunction.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetConfigurationSetting(string settingName)
        {
            var settingValue = System.Environment.GetEnvironmentVariable(settingName);
            if(settingValue is null)
            {
                throw new MissingMemberException($"Setting {settingName} has no value.");                
            }
            return settingValue;
        }        

        public string ReferentialStorageConnectionString => GetConfigurationSetting("ReferentialStorageConnectionString");
        public string ReferentialContainerName => GetConfigurationSetting("ReferentialContainerName");
        public string ReferentialBlobName => GetConfigurationSetting("ReferentialBlobName");
        public string KeyVaultName => GetConfigurationSetting("KeyVaultName");
        public string LogStorageConnectionString => GetConfigurationSetting("LogStorageConnectionString");
        public string LogContainerName => GetConfigurationSetting("LogContainerName");
        public string LogTableName => GetConfigurationSetting("LogTableName");
        public string VaultName => GetConfigurationSetting("KeyVaultName");        
    }
}