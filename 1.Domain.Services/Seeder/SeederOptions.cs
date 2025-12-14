using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

public class SeederOptions
{
    private readonly SeederService _seedService;

    public SeederOptions(SeederService seedService)
    {
        _seedService = seedService;
    }

    public void AddMocker<TInterface, TInstance>(Func<SeedGenerator, TInstance, TInstance> mocker)
        where TInterface : class
        where TInstance : new()
    {
        if (!typeof(TInterface).IsInterface)
            throw new ArgumentException($"Type {typeof(TInterface).Name} must be an interface");

        _seedService._typeMockers[typeof(TInterface)] = (seeder) => mocker(seeder, new TInstance());
    }
    public void AddMocker<TInstance>(Func<SeedGenerator, TInstance, TInstance> mocker)
        where TInstance : new()
    {
        if (!typeof(TInstance).IsClass)
            throw new ArgumentException($"Type {typeof(TInstance).Name} must be a class");

        _seedService._typeMockers[typeof(TInstance)] = (seeder) => mocker(seeder, new TInstance());
    }
}
