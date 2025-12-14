using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Security;

namespace Services.Encryptions;

// The builder pattern follows conventions used by libraries like Entity Framework Core (AddDbContext().UseSqlServer())
// and others in the .NET ecosystem.
public class EncryptionBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<EncryptionOptions>> _configureActions = new List<Action<EncryptionOptions>>();
    public EncryptionBuilder(IServiceCollection services)
    {
        _services = services;
        
        // Register the EncryptionService with deferred configuration
        _services.AddTransient<EncryptionService>(sp =>
        {
            var encryptionEngine = sp.GetRequiredService<EncryptionEngine>();
            var encryptionService = new EncryptionService(encryptionEngine);
            if (_configureActions.Any())
            {
                var options = new EncryptionOptions(encryptionService);
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }
            return encryptionService;
        });
    }

    public EncryptionBuilder Configure(Action<EncryptionOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
