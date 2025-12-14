# Student Assignment: Implementing Encryption Service with Functional Programming Patterns

## Overview
In this assignment, you will implement a **EncryptionService** that separates concerns by moving obfuscation logic from domain models into a configurable service layer. You will leverage **functional programming concepts**, the **Builder pattern**, and the **Options pattern** to create a flexible, type-safe encryption and obfuscation system.

## Learning Objectives
- Understand **separation of concerns** between domain models and cross-cutting services
- Apply **functional programming** concepts: higher-order functions, function composition, and pure functions
- Implement the **Builder pattern** for fluent service configuration
- Use the **Options pattern** for deferred configuration in dependency injection
- Work with **generic type parameters** and **type dictionaries** in C#
- Apply **LINQ** for functional data transformations

---

## Current State Analysis

### The Problem
Currently, the `CreditCard` model has mixed responsibilities:

```csharp
public class CreditCard : ICreditCard
{
    public string Number { get; set; }
    public string EnryptedToken { get; set; }
    
    // ❌ Business logic mixed with domain model
    public CreditCard EnryptAndObfuscate(Func<CreditCard, string> encryptor)
    {
        this.EnryptedToken = encryptor(this);
        
        // Obfuscation logic embedded in the model
        string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        string replacement = "$1**** **** **** $4";
        this.Number = Regex.Replace(Number, pattern, replacement);
        
        this.ExpirationYear = "**";
        this.ExpirationMonth = "**";
        
        return this;
    }
}
```

**Issues:**
1. **Violates Single Responsibility Principle** - models should represent data, not contain business logic
2. **Hard to test** - obfuscation logic is tightly coupled to the model
3. **Not reusable** - each model must implement its own obfuscation
4. **Difficult to configure** - obfuscation rules are hardcoded

---

## Target Architecture

### Goal
Move obfuscation logic from models to a configurable service that:
- Accepts **functions as configuration** (higher-order functions)
- Uses a **type-based registry** to store obfuscation strategies
- Provides a **fluent API** for configuration via the Builder pattern
- Integrates seamlessly with **Dependency Injection**

### Class Structure
```
1.Domain.Services/Encryptions/
├── EncryptionService.cs       # Core service with type-based obfuscation registry
├── EncryptionOptions.cs       # Options for registering obfuscators
├── EncryptionBuilder.cs       # Builder for fluent configuration
└── EncryptionExtensions.cs    # Extension method for DI registration

0.App.AppWorker/Obfuscation/
└── ObfuscateEmployee.cs       # Employee-specific obfuscation configuration
```

---

## Functional Programming Concepts in This Assignment

### 1. Higher-Order Functions
Functions that accept other functions as parameters or return functions.

**Example from the assignment:**
```csharp
// This method accepts a function as a parameter
public void AddObfuscator<TInterface, TInstance>(
    Func<EncryptionService, TInterface, TInterface> obfuscator)
{
    // Store the function for later execution
    _encryptionService._typeObfuscators[typeof(TInterface)] = obfuscator;
}
```

### 2. Function Composition
Combining functions to create new functionality.

**Example:**
```csharp
// The obfuscator function can call other functions on the service
options.AddObfuscator<IEmployee, Employee>((encryptionService, emp) =>
{
    // Compose: use the service's ObfuscateMany to handle nested objects
    emp.CreditCards = encryptionService.ObfuscateMany<ICreditCard>(emp.CreditCards).ToList();
    return emp;
});
```

### 3. Pure Functions
Functions that always produce the same output for the same input without side effects.

**Example:**
```csharp
// This lambda is a pure transformation function
(_, cc) =>
{
    cc.Number = Regex.Replace(cc.Number, pattern, replacement);
    return cc;
}
```

### 4. Deferred Execution
Configuration is stored and executed later during service instantiation.

**Example:**
```csharp
// Configuration is stored, not executed
_configureActions.Add(configure);

// Later, during service creation:
foreach (var configureAction in _configureActions)
{
    configureAction(options); // Now execute
}
```

---

## Assignment Tasks

### Task 1: Implement `EncryptionOptions.cs`

Complete the `AddObfuscator` methods that register obfuscation functions in the service's type dictionary.

**Starter code:**
```csharp
public class EncryptionOptions
{
    private readonly EncryptionService _encryptionService;

    public EncryptionOptions(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void AddObfuscator<TInterface, TInstance>(
        Func<EncryptionService, TInterface, TInterface> obfuscator)
        where TInterface : class
        where TInstance : TInterface, new()
    {
        if (!typeof(TInterface).IsInterface)
            throw new ArgumentException($"Type {typeof(TInterface).Name} must be an interface");

        // TODO: 
        // 1. Store the obfuscator function in _encryptionService._typeObfuscators
        //    Key: typeof(TInterface)
        //    Value: A function that casts source to TInterface and applies obfuscator
        // 2. Add an AbstractConverter<TInterface, TInstance> to _encryptionService._abstractConverters
    }

    public void AddObfuscator<TInstance>(
        Func<EncryptionService, TInstance, TInstance> obfuscator)
        where TInstance : class, new()
    {
        // TODO:
        // Store the obfuscator function in _encryptionService._typeObfuscators
        // Key: typeof(TInstance)
        // Value: A function that casts source to TInstance and applies obfuscator
    }
}
```

**Hints:**
- The dictionary signature is: `Dictionary<Type, Func<EncryptionService, object, object>>`
- You need to wrap the typed obfuscator in a function that handles `object` types
- Use lambda expressions to perform the type casting

**Expected solution:**
```csharp
_encryptionService._typeObfuscators[typeof(TInterface)] = 
    (encryptionService, source) => obfuscator(encryptionService, (TInterface)source);

_encryptionService._abstractConverters.Add(
    new Configuration.AbstractConverter<TInterface, TInstance>());
```

---

### Task 2: Implement `EncryptionBuilder.cs`

Complete the builder constructor to register the `EncryptionService` with deferred configuration.

**Starter code:**
```csharp
public class EncryptionBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<EncryptionOptions>> _configureActions = new();
    
    public EncryptionBuilder(IServiceCollection services)
    {
        _services = services;
        
        // TODO: Register EncryptionService with deferred configuration
        // 1. Use _services.AddTransient<EncryptionService>()
        // 2. Accept a ServiceProvider (sp) to resolve dependencies
        // 3. Get EncryptionEngine from sp
        // 4. Create new EncryptionService instance
        // 5. Apply all _configureActions to configure the service
        // 6. Return the configured service
    }

    public EncryptionBuilder Configure(Action<EncryptionOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
```

**Functional Programming Concept:** This demonstrates **deferred execution** - configuration actions are collected and executed later during service instantiation.

**Hints:**
- Use `sp.GetRequiredService<EncryptionEngine>()` to get dependencies
- Check if `_configureActions.Any()` before applying configurations
- Create `EncryptionOptions` and iterate through `_configureActions`

**Expected solution:**
```csharp
_services.AddTransient<EncryptionService>(sp =>
{
    var encryptionEngine = sp.GetRequiredService<EncryptionEngine>();
    var encryptionService = new EncryptionService(encryptionEngine);
    
    if (_configureActions.Any())
    {
        var options = new EncryptionOptions(encryptionService);
        foreach (var configureAction in _configureActions)
        {
            configureAction(options);
        }
    }
    
    return encryptionService;
});
```

---

### Task 3: Implement `EncryptionService.cs` Methods

Complete the core encryption and obfuscation methods using the registered obfuscators.

**Starter code:**
```csharp
public class EncryptionService
{
    private readonly EncryptionEngine _encryptionEngine;
    internal readonly Dictionary<Type, Func<EncryptionService, object, object>> _typeObfuscators = new();
    internal readonly List<JsonConverter> _abstractConverters = new();

    public EncryptionService(EncryptionEngine EncryptionEngine)
    {
        _encryptionEngine = EncryptionEngine;
    }
    
    public (T obfuscatedObject, string encryptedToken) EncryptAndObfuscate<T>(T source) 
        where T : class
    {
        // TODO:
        // 1. Look up the obfuscator function from _typeObfuscators using typeof(T)
        // 2. Encrypt the source using _encryptionEngine.AesEncryptToBase64(source)
        // 3. Apply the obfuscator function (pass 'this' and source)
        // 4. Return tuple of (obfuscated object, encrypted token)
        // 5. Throw KeyNotFoundException if no obfuscator found
    }

    public T Obfuscate<T>(T source) where T : class
    {
        // TODO: Similar to above but only obfuscate, don't encrypt
    }

    public IEnumerable<(T obfuscatedObject, string encryptedToken)> EncryptAndObfuscateMany<T>(
        IEnumerable<T> sources) where T : class
    {
        // TODO: Use LINQ Select to apply EncryptAndObfuscate to each source
    }

    public IEnumerable<T> ObfuscateMany<T>(IEnumerable<T> sources) where T : class
    {
        // TODO: Use LINQ Select to apply Obfuscate to each source
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
```

**Functional Programming Concepts:**
- **Dictionary lookup with TryGetValue** - functional pattern matching
- **LINQ Select** - functional map operation over collections
- **Tuple returns** - functional pattern for multiple return values

**Hints:**
- Use `_typeObfuscators.TryGetValue(typeof(T), out var obfuscateFunc)`
- The obfuscator returns `object`, so cast back to `T`
- LINQ: `sources.Select(source => EncryptAndObfuscate(source))`

---

### Task 4: Implement `ObfuscateEmployee.cs`

Create the employee-specific obfuscation configuration as an extension method.

**Starter code:**
```csharp
public static partial class EncryptionObfuscation
{
    public static EncryptionBuilder ObfuscateEmployee(this EncryptionBuilder builder)
    {       
        builder.Configure(options => { 
            // TODO: Configure credit card obfuscation
            options.AddObfuscator<ICreditCard, CreditCard>((_, cc) =>
            {
                // 1. Obfuscate card number to show only first and last 4 digits
                //    Pattern: @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b"
                //    Replacement: "$1**** **** **** $4"
                // 2. Set ExpirationYear and ExpirationMonth to "**"
                // 3. Return the obfuscated credit card
            });
            
            // TODO: Configure employee obfuscation
            options.AddObfuscator<IEmployee, Employee>((encryptionService, emp) =>
            {
                // 1. Set LastName to "***"
                // 2. Set HireDate to default
                // 3. Set Role to WorkRole.Undefined
                // 4. Obfuscate credit cards using encryptionService.ObfuscateMany<ICreditCard>()
                // 5. Return the obfuscated employee
            });
        });
        return builder;
    }
}
```

**Functional Programming Concept:** The `(encryptionService, emp) =>` lambda demonstrates **function composition** - you use the `encryptionService` parameter to call `ObfuscateMany`, composing the employee obfuscation with credit card obfuscation.

---

### Task 5: Configure in `Program.cs`

Register the encryption service with the obfuscation configuration.

**Add to Program.cs:**
```csharp
using AppWorker.Obfuscation;
using Services.Encryptions;

// After AddSeeder() line:
builder.Services.AddEncryptionService().ObfuscateEmployee();
```

**Fluent API Pattern:** Notice how the Builder pattern enables chaining:
```csharp
builder.Services
    .AddEncryptionService()  // Returns EncryptionBuilder
    .ObfuscateEmployee();    // Extension method on EncryptionBuilder
```

---

## Testing Your Implementation

### Update `EmployeesService.cs`

Replace the old obfuscation with the new service:

**Before:**
```csharp
encryptedCards.Add(
    ((CreditCard)c).EnryptAndObfuscate(_encryptions.AesEncryptToBase64<CreditCard>)
);
```

**After:**
```csharp
// Test single credit card
var encryptedCard = _encryptionService.EncryptAndObfuscate<ICreditCard>(c);
encryptedCards.Add(encryptedCard.obfuscatedObject);

// Or test employee
var encryptedEmployee = _encryptionService.EncryptAndObfuscate<IEmployee>(emp);
var decryptedEmployee = _encryptionService.Decrypt<Employee>(encryptedEmployee.encryptedToken);
```

---

## Model Transformation: CreditCard as a Clean POCO

### Before: Mixed Responsibilities (Current Branch)

In the starting branch, the `CreditCard` model contains both **data representation** and **business logic** for encryption and obfuscation:

```csharp
public class CreditCard : ICreditCard
{
    public Guid CreditCardId { get; set; }
    public CardIssuer Issuer { get; set; }
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }
    public string EnryptedToken { get; set; }

    // ❌ BUSINESS LOGIC IN DOMAIN MODEL
    public CreditCard EnryptAndObfuscate(Func<CreditCard, string> encryptor)
    {
        this.EnryptedToken = encryptor(this);

        string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        string replacement = "$1**** **** **** $4";
        this.Number = Regex.Replace(Number, pattern, replacement);

        this.ExpirationYear = "**";
        this.ExpirationMonth = "**";

        return this;
    }

    public ICreditCard Decrypt(Func<string, JsonSerializerSettings, ICreditCard> decryptor, 
        string encryptedToken)
    {
        return decryptor(encryptedToken, new JsonSerializerSettings
        {
            Converters = { new Configuration.AbstractConverter<ICreditCard, CreditCard>() }
        });
    }
}
```

**Problems:**
- Violates **Single Responsibility Principle** - model should only represent data
- Tight coupling to encryption/obfuscation logic
- Hard to test independently
- Difficult to change obfuscation rules without modifying the model
- Mixes domain concerns with cross-cutting concerns

### After: Clean POCO (Final Branch 2-encryption-assignment-answer)

After completing the assignment, `CreditCard` becomes a **pure data model**:

```csharp
public class CreditCard : ICreditCard
{
    public Guid CreditCardId { get; set; }
    public CardIssuer Issuer { get; set; }
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }
    public string EnryptedToken { get; set; }

    // ✅ CLEAN CONSTRUCTORS ONLY
    public CreditCard() {}
    public CreditCard(ICreditCard original)
    {
        CreditCardId = original.CreditCardId;
        Issuer = original.Issuer;
        Number = original.Number;
        ExpirationYear = original.ExpirationYear;
        ExpirationMonth = original.ExpirationMonth;
        EnryptedToken = original.EnryptedToken;
    }
}
```

**Benefits:**
- ✅ **Single Responsibility** - only represents credit card data
- ✅ **No infrastructure dependencies** - no references to encryption or regex
- ✅ **Easy to test** - simple data container
- ✅ **Flexible** - obfuscation logic can be changed without touching the model
- ✅ **Follows domain-driven design** - pure domain model

### Where Did the Logic Go?

The encryption and obfuscation logic moved to **`0.App.AppWorker/Obfuscation/ObfuscateEmployee.cs`**:

```csharp
public static EncryptionBuilder ObfuscateEmployee(this EncryptionBuilder builder)
{       
    builder.Configure(options => { 
        options.AddObfuscator<ICreditCard, CreditCard>((_, cc) =>
        {
            // Obfuscation logic now here, not in the model!
            string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
            string replacement = "$1**** **** **** $4";
            cc.Number = Regex.Replace(cc.Number, pattern, replacement);

            cc.ExpirationYear = "**";
            cc.ExpirationMonth = "**";

            return cc;
        });
    });
    return builder;
}
```

**This demonstrates:**
- **Separation of Concerns** - models vs. services
- **Dependency Inversion** - models don't depend on infrastructure
- **Open/Closed Principle** - can add new obfuscation strategies without modifying models
- **Configuration over Convention** - behavior is configured, not hardcoded

---

## Key Functional Programming Patterns Summary

### 1. **Higher-Order Functions**
```csharp
// Functions accepting functions as parameters
public void AddObfuscator<T>(Func<EncryptionService, T, T> obfuscator)

// Storing functions in data structures
_typeObfuscators[typeof(T)] = obfuscatorFunction;
```

### 2. **Function Composition**
```csharp
// Inner function uses outer function
(encryptionService, emp) => {
    emp.CreditCards = encryptionService.ObfuscateMany<ICreditCard>(emp.CreditCards).ToList();
    return emp;
}
```

### 3. **Deferred Execution**
```csharp
// Configuration stored for later execution
_configureActions.Add(configure);  // Store
configureAction(options);          // Execute later
```

### 4. **LINQ Functional Operations**
```csharp
// Map operation
sources.Select(source => EncryptAndObfuscate(source))

// Filter + Check
if (_configureActions.Any()) { ... }
```

### 5. **Type-Safe Generic Programming**
```csharp
// Generic constraints for type safety
public void AddObfuscator<TInterface, TInstance>()
    where TInterface : class
    where TInstance : TInterface, new()
```

### 6. **Immutability & Pure Functions**
```csharp
// Lambda that transforms input to output without external dependencies
(_, cc) => {
    cc.Number = Regex.Replace(cc.Number, pattern, replacement);
    return cc;
}
```

---

## Reflection Questions

1. **Separation of Concerns:** How does moving obfuscation logic from models to services improve maintainability?

2. **Functional Programming:** How do higher-order functions (functions as parameters) make the code more flexible than traditional inheritance?

3. **Builder Pattern:** What advantages does the fluent API provide over passing configuration objects directly?

4. **Type Safety:** How do generic type parameters and constraints prevent runtime errors?

5. **Composition vs Inheritance:** The employee obfuscator calls the credit card obfuscator. How does this demonstrate composition over inheritance?

---

## Expected Behavior

After completing the assignment:

1. **Models are clean** - no business logic in `CreditCard` or `Employee`
2. **Flexible configuration** - obfuscation rules defined separately from models
3. **Reusable** - same pattern can be applied to any type
4. **Type-safe** - compiler enforces correct types
5. **Testable** - obfuscation logic can be tested independently
6. **Functional** - heavy use of functions as first-class citizens

---

## Bonus Challenges

1. **Add logging** to track which types are being obfuscated
2. **Implement caching** for frequently obfuscated objects
3. **Add conditional obfuscation** based on user roles
4. **Create a fluent API** for configuring obfuscation rules (e.g., `.HideLastName().HideHireDate()`)
5. **Implement async obfuscation** for heavy operations

---

## Submission Checklist

- [ ] `EncryptionOptions.cs` - Both `AddObfuscator` methods implemented
- [ ] `EncryptionBuilder.cs` - Service registration with deferred configuration
- [ ] `EncryptionService.cs` - All 4 obfuscation/encryption methods implemented
- [ ] `ObfuscateEmployee.cs` - Credit card and employee obfuscators configured
- [ ] `Program.cs` - Service registered with `.AddEncryptionService().ObfuscateEmployee()`
- [ ] Code compiles without errors
- [ ] Application runs and employees are properly obfuscated
- [ ] Reflection questions answered

---

## Additional Resources

- [Func Delegate Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)
- [Builder Pattern](https://refactoring.guru/design-patterns/builder)
- [Options Pattern in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
- [Functional Programming in C#](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)

Good luck! 🚀
