using Messaging.Contract.Repositories;
using Messaging.Contract.Services;
using Messaging.Data.Repositories;
using Messaging.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Data
{
    public static class DataBootstrapper
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<StorageClientFactory>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IApplicationRepository, ApplicationRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<IMessageService, MessageService>()
                .AddScoped<IFileBlobRepository, FileBlobRepository>()
                .AddScoped<IFileMetadataRepository, FileMetadataRepository>()
                .AddScoped<IFileService, FileService>();
        }
    }
}