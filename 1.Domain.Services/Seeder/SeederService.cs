using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace Services.Seeder;

public class SeederService
{
    private readonly SeedGenerator _seeder = new SeedGenerator();
    internal readonly Dictionary<Type, Func<SeedGenerator, object>> _typeMockers = new Dictionary<Type, Func<SeedGenerator, object>>();

    public TInterface Mock<TInterface>()
        where TInterface : class
    {
        if (_typeMockers.TryGetValue(typeof(TInterface), out var mockerFunc))
        {
            return (TInterface)mockerFunc(_seeder);
        }
        throw new KeyNotFoundException($"No mocker found for type {typeof(TInterface).FullName}");
    }
    public IEnumerable<TInterface> MockMany<TInterface>(int nrInstances)
        where TInterface : class
    {
        if (_typeMockers.TryGetValue(typeof(TInterface), out var mockerFunc))
        {
            return Enumerable.Repeat(0,nrInstances).Select(_ => (TInterface)mockerFunc(_seeder));
        }
        throw new KeyNotFoundException($"No mocker found for type {typeof(TInterface).FullName}");
    }

}
