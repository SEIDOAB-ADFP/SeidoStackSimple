using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Encryptions;

// The builder pattern follows conventions used by libraries like Entity Framework Core (AddDbContext().UseSqlServer())
// and others in the .NET ecosystem.
public static class EncryptionsExtensions
{
    public static EncryptionBuilder AddEncryptionService(this IServiceCollection serviceCollection)
    {
        return new EncryptionBuilder(serviceCollection);
    }
}