using Messaging.Contract.Repositories;
using Messaging.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Data
{
    public static class DataBootstrapper
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<TableClientFactory>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IAppSpaceRepository, AppSpaceRepository>();
        }
    }
}