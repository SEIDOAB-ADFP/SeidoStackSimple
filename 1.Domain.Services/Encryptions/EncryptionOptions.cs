using Models;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Encryptions;

public class EncryptionOptions
{
    private readonly EncryptionService _encryptionService;

    public EncryptionOptions(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void AddObfuscator<TInterface, TInstance>(Func<EncryptionService, TInterface, TInterface> obfuscator)
        where TInterface : class
        where TInstance : TInterface, new()

    {
        if (!typeof(TInterface).IsInterface)
            throw new ArgumentException($"Type {typeof(TInterface).Name} must be an interface");

        _encryptionService._typeObfuscators[typeof(TInterface)] = (encryptionService, source) => obfuscator(encryptionService, (TInterface)source);
        _encryptionService._abstractConverters.Add(new Configuration.AbstractConverter<TInterface, TInstance>());
    }

    public void AddObfuscator<TInstance>(Func<EncryptionService, TInstance, TInstance> obfuscator)
        where TInstance : class, new()
    {
        _encryptionService._typeObfuscators[typeof(TInstance)] = (encryptionService, source) => obfuscator(encryptionService, (TInstance)source);
    }
}