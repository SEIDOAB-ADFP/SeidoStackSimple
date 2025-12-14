using System.Text.RegularExpressions;
using Models.Employees.Interfaces;
using Security;
using Newtonsoft.Json;
using Models.Employees;

namespace Services.Encryptions;

public class EncryptionService
{
    private readonly EncryptionEngine _encryptionEngine;
    internal readonly Dictionary<Type, Func<EncryptionService, object, object>> _typeObfuscators = new();
    internal readonly List<JsonConverter> _abstractConverters = new();


    public EncryptionService(EncryptionEngine EncryptionEngine)
    {
        _encryptionEngine = EncryptionEngine;

        // Test 1 the Dictionary with a built-in obfuscator for ICreditCard
        // _typeObfuscators[typeof(ICreditCard)] = (source) =>
        // {
        //     var creditCard = source as ICreditCard;

        //     string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        //     string replacement = "$1**** **** **** $4";
        //     creditCard.Number = Regex.Replace(creditCard.Number, pattern, replacement);

        //     creditCard.ExpirationYear = "**";
        //     creditCard.ExpirationMonth = "**";

        //     return creditCard;
        // };

        // Test 2 the Dictionary using Options.AddObfuscator
        // new EncryptionOptions(this).AddObfuscator<ICreditCard, CreditCard>((service, cc) =>
        // {
        //     string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        //     string replacement = "$1**** **** **** $4";
        //     cc.Number = Regex.Replace(cc.Number, pattern, replacement);

        //     cc.ExpirationYear = "**";
        //     cc.ExpirationMonth = "**";

        //     return cc;
        // });
        // new EncryptionOptions(this).AddObfuscator<IEmployee, Employee>((_, emp) =>
        // {
        //     emp.LastName = "***";
        //     emp.HireDate = default;
        //     emp.Role = WorkRole.Undefined;

        //     emp.CreditCards = ObfuscateMany<ICreditCard>(emp.CreditCards).ToList<ICreditCard>();
        //     return emp;
        // });
        // new EncryptionOptions(this).AddObfuscator<Employee2 >((encryptionService, emp) =>
        // {
        //     emp.LastName = "***";
        //     emp.HireDate = default;
        //     emp.Role = WorkRole.Undefined;

        //     emp.CreditCards = encryptionService.ObfuscateMany<CreditCard2>(emp.CreditCards).ToList<CreditCard2>();
        //     return emp;
        // });
        // new EncryptionOptions(this).AddObfuscator<CreditCard2>((_, cc) =>
        // {
        //     string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        //     string replacement = "$1**** **** **** $4";
        //     cc.Number = Regex.Replace(cc.Number, pattern, replacement);

        //     cc.ExpirationYear = "**";
        //     cc.ExpirationMonth = "**";

        //     return cc;
        // });
        
    }
    
    public (T obfuscatedObject, string encryptedToken) EncryptAndObfuscate<T>(T source) where T : class
    {
        if (_typeObfuscators.TryGetValue(typeof(T), out var obfuscateFunc))
        {
            var encryptedToken = _encryptionEngine.AesEncryptToBase64(source);
            var obfuscatedObject = obfuscateFunc(this, source);

            return ((T)obfuscatedObject, encryptedToken);
        }
        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public T Obfuscate<T>(T source) where T : class
    {
        if (_typeObfuscators.TryGetValue(typeof(T), out var obfuscateFunc))
        {
            var obfuscatedObject = obfuscateFunc(this, source);
            return (T)obfuscatedObject;
        }
        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public IEnumerable<(T obfuscatedObject, string encryptedToken)> EncryptAndObfuscateMany<T>(IEnumerable<T> sources) where T : class
    {
        if (_typeObfuscators.TryGetValue(typeof(T), out var obfuscateFunc))
        {
            return sources.Select(source => EncryptAndObfuscate(source));
        }
        throw new KeyNotFoundException($"No obfuscator found for type {typeof(T).FullName}");
    }

    public IEnumerable<T> ObfuscateMany<T>(IEnumerable<T> sources) where T : class
    {
        if (_typeObfuscators.TryGetValue(typeof(T), out var obfuscateFunc))
        {
            return sources.Select(source => Obfuscate(source));
        }
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