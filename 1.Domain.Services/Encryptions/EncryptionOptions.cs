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

        //Your code here to register the obfuscator and abstract converter
    }

    public void AddObfuscator<TInstance>(Func<EncryptionService, TInstance, TInstance> obfuscator)
        where TInstance : class, new()
    {
        //Your code here to register the obfuscator
    }
}