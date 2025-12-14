using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

// The builder pattern follows conventions used by libraries like Entity Framework Core (AddDbContext().UseSqlServer())
// and others in the .NET ecosystem.
public class SeederBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<SeederOptions>> _configureActions = new List<Action<SeederOptions>>();

    public SeederBuilder(IServiceCollection services)
    {
        _services = services;
        
        // Register the SeedService with deferred configuration
        _services.AddSingleton<SeederService>(sp =>
        {
            var seedService = new SeederService();
            if (_configureActions.Any())
            {
                var options = new SeederOptions(seedService);
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }
            return seedService;
        });
    }

    public SeederBuilder Configure(Action<SeederOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
