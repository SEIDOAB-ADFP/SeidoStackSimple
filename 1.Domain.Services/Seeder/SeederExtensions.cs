using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

// The builder pattern follows conventions used by libraries like Entity Framework Core (AddDbContext().UseSqlServer())
// and others in the .NET ecosystem.
public static class SeederExtensions
{
    public static SeederBuilder AddSeeder(this IServiceCollection serviceCollection)
    {
        return new SeederBuilder(serviceCollection);
    }
}