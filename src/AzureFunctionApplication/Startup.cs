using System;
using Azure.Identity;
using AzureFunctionApplication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureFunctionApplication
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var builtConfig = builder.ConfigurationBuilder.Build();
            var keyVaultEndpoint = builtConfig["VaultUri"];
            builder.ConfigurationBuilder.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
        }
    }
}
