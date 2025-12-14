using System.Text.RegularExpressions;
using Models.Employees.Interfaces;
using Security;
using Newtonsoft.Json;
using Models.Employees;

namespace Services.Encryptions;

public class EncryptionService
{
    private readonly EncryptionEngine _encryptionEngine;

    //Dictionary to hold obfuscators for different types registered using EncryptionOptions methods 
    //AddObfuscator<TInterface, TInstance> and AddObfuscator<TInstance>
    internal readonly Dictionary<Type, Func<EncryptionService, object, object>> _typeObfuscators = new();
    internal readonly List<JsonConverter> _abstractConverters = new();


    public EncryptionService(EncryptionEngine EncryptionEngine)
    {
        _encryptionEngine = EncryptionEngine;        
    }
    
    public (T obfuscatedObject, string encryptedToken) EncryptAndObfuscate<T>(T source) where T : class
    {
        //Your code here to get obfuscation function from Dictionary and encrypt and obfuscate

        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public T Obfuscate<T>(T source) where T : class
    {
        //Your code here to get obfuscation function from Dictionary and obfuscate

        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public IEnumerable<(T obfuscatedObject, string encryptedToken)> EncryptAndObfuscateMany<T>(IEnumerable<T> sources) where T : class
    {
        //Your code here to get obfuscation function from Dictionary and encrypt and obfuscate

        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public IEnumerable<T> ObfuscateMany<T>(IEnumerable<T> sources) where T : class
    {
        //Your code here to get obfuscation function from Dictionary and obfuscate

        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public T Decrypt<T>(string encryptedToken)
    {
        JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Converters = _abstractConverters
        };

        return _encryptionEngine.AesDecryptFromBase64<T>(encryptedToken, _jsonSettings);
    }
}