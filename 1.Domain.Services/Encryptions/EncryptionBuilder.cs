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
        
        // Your code here to register the EncryptionService with deferred configuration
    }

    public EncryptionBuilder Configure(Action<EncryptionOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
