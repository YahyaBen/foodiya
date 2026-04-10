using Foodiya.Domain.Configuration;
using Foodiya.Domain.Interfaces.Core;
using Foodiya.Infrastructure.Extensions.Data;
using Foodiya.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foodiya.Infrastructure.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
            services.AddRepositories(configuration, isDevelopment);

            // Azure Blob Storage
            // Supports: appsettings AzureStorage:ConnectionString OR Service Connector env var
            services.Configure<BlobStorageOptions>(opts =>
            {
                configuration.GetSection(BlobStorageOptions.SectionName).Bind(opts);

                if (string.IsNullOrWhiteSpace(opts.ConnectionString))
                    opts.ConnectionString = configuration["AZURE_STORAGEBLOB_CONNECTIONSTRING"] ?? "";
            });
            services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
        }
    }
}
