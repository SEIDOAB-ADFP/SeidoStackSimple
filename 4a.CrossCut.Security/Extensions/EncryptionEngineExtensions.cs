using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Security.Options;

namespace Security.Extensions;

public static class EncryptionEngineExtensions
{
    public static IServiceCollection AddEncryptionEngine(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<AesEncryptionOptions>(
            options => configuration.GetSection(AesEncryptionOptions.Position).Bind(options));
        serviceCollection.AddTransient<EncryptionEngine>();

        return serviceCollection;
    }
}