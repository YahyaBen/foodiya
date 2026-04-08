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
            // Add Other Services as "Azure AD", "Mail Jet", "DocuSign" ...
        }
    }
}
