# Pattern Template for Future Services

This document provides a reusable architectural pattern template extracted from the SeederService and EncryptionService implementations. It demonstrates how to create new services following the same four-component architecture: Service, Options, Builder, and Extensions.

---

## Table of Contents
1. [Generic Pattern Template](#generic-pattern-template)
2. [GreetingService Implementation Example](#greetingservice-implementation-example)
3. [Step-by-Step Implementation Guide](#step-by-step-implementation-guide)
4. [Pattern Application Checklist](#pattern-application-checklist)

---

## Generic Pattern Template

Based on the SeederService and EncryptionService implementations, here's a reusable template:

```csharp
// 1. Service
public class [Feature]Service
{
    private readonly [Dependency] _dependency;
    internal readonly Dictionary<Type, Func<[Context], object>> _typeStrategies = new();

    public [Feature]Service([Dependency] dependency)
    {
        _dependency = dependency;
    }
    
    public T Execute<T>(T source) where T : class
    {
        if (_typeStrategies.TryGetValue(typeof(T), out var strategyFunc))
        {
            return (T)strategyFunc(source);
        }
        throw new KeyNotFoundException($"No strategy found for type {typeof(T).FullName}");
    }
}

// 2. Options
public class [Feature]Options
{
    private readonly [Feature]Service _service;

    public [Feature]Options([Feature]Service service)
    {
        _service = service;
    }

    public void AddStrategy<TInterface, TInstance>(
        Func<[Context], TInterface, TInterface> strategy)
        where TInterface : class
        where TInstance : TInterface, new()
    {
        _service._typeStrategies[typeof(TInterface)] = 
            (context, source) => strategy(context, (TInterface)source);
    }
}

// 3. Builder
public class [Feature]Builder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<[Feature]Options>> _configureActions = new();

    public [Feature]Builder(IServiceCollection services)
    {
        _services = services;
        
        _services.Add[Lifetime]<[Feature]Service>(sp =>
        {
            var dependency = sp.GetRequiredService<[Dependency]>();
            var service = new [Feature]Service(dependency);
            if (_configureActions.Any())
            {
                var options = new [Feature]Options(service);
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }
            return service;
        });
    }

    public [Feature]Builder Configure(Action<[Feature]Options> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}

// 4. Extensions
public static class [Feature]Extensions
{
    public static [Feature]Builder Add[Feature](this IServiceCollection serviceCollection)
    {
        return new [Feature]Builder(serviceCollection);
    }
}
```

### Template Placeholders

- **[Feature]**: The name of your service domain (e.g., Greeting, Seeder, Encryption)
- **[Dependency]**: External dependencies needed by your service (e.g., ILogger, EncryptionEngine)
- **[Context]**: The context passed to strategy functions (e.g., SeedGenerator, EncryptionService, entity object)
- **[Lifetime]**: DI lifetime (AddSingleton, AddTransient, AddScoped)

---

## GreetingService Implementation Example

The GreetingService demonstrates the pattern template applied to a simple greeting message use case. It shows how the same four-component architecture can be reused for a completely different domain.

### Overview

**Purpose:** Generate customizable greeting messages for different entity types  
**Branch:** `main` (integrated example)  
**Location:** `1.Domain.Services/Greetings/`  
**Dependencies:** `ILogger<GreetingService>`  
**DI Lifetime:** Singleton  

---

## Step-by-Step Implementation Guide

### Step 1: Create the Core Service (`GreetingService.cs`)

**Location:** `1.Domain.Services/Greetings/GreetingService.cs`

```csharp
using Microsoft.Extensions.Logging;

namespace Services.Greetings;

public class GreetingService
{
    private readonly ILogger<GreetingService> _logger;
    internal readonly Dictionary<Type, Func<object, string>> _typeGreeters = new();

    public GreetingService(ILogger<GreetingService> logger)
    {
        _logger = logger;
    }
    
    public string Greet<T>(T entity) where T : class
    {
        if (_typeGreeters.TryGetValue(typeof(T), out var greeterFunc))
        {
            return greeterFunc(entity);
        }
        
        // Default greeting if no custom greeter registered
        return $"Hello, {entity}!";
    }
    
    public IEnumerable<string> GreetMany<T>(IEnumerable<T> entities) where T : class
    {
        return entities.Select(entity => Greet(entity));
    }
}
```

**Pattern Mapping:**
- **[Feature]Service** → `GreetingService`
- **[Dependency]** → `ILogger<GreetingService>`
- **Dictionary<Type, Func<[Context], object>>** → `Dictionary<Type, Func<object, string>>`
- **Execute<T>()** → `Greet<T>()`
- **ExecuteMany<T>()** → `GreetMany<T>()`

**Key Design Decisions:**
1. **Registry Type:** `Dictionary<Type, Func<object, string>>`
   - Key: Type of entity to greet
   - Value: Function that takes entity and returns greeting string
2. **Default Behavior:** Returns generic greeting if no custom greeter registered
3. **Simple Operations:** No transformation needed, just message generation

---

### Step 2: Create the Options Class (`GreetingOptions.cs`)

**Location:** `1.Domain.Services/Greetings/GreetingOptions.cs`

```csharp
namespace Services.Greetings;

public class GreetingOptions
{
    private readonly GreetingService _service;

    public GreetingOptions(GreetingService service)
    {
        _service = service;
    }

    public void AddGreeter<T>(Func<T, string> greeter)
        where T : class
    {
        _service._typeGreeters[typeof(T)] = 
            (entity) => greeter((T)entity);
    }
}
```

**Pattern Mapping:**
- **[Feature]Options** → `GreetingOptions`
- **AddStrategy<T>()** → `AddGreeter<T>()`
- **Strategy Function Signature:** `Func<T, string>` (takes entity, returns greeting)

**Key Design Decisions:**
1. **Simplified Signature:** Only one generic type parameter (no TInterface/TInstance split)
   - Simpler use case than Seeder/Encryption
   - No need for interface abstraction
2. **Type Casting Wrapper:** Wraps typed function in object-based function for dictionary storage
3. **No Composition:** Greeters don't need to call other greeters

---

### Step 3: Create the Builder Class (`GreetingBuilder.cs`)

**Location:** `1.Domain.Services/Greetings/GreetingBuilder.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Services.Greetings;

public class GreetingBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<GreetingOptions>> _configureActions = new();

    public GreetingBuilder(IServiceCollection services)
    {
        _services = services;
        
        _services.AddSingleton<GreetingService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<GreetingService>>();
            var service = new GreetingService(logger);
            
            if (_configureActions.Any())
            {
                var options = new GreetingOptions(service);
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }
            
            return service;
        });
    }

    public GreetingBuilder Configure(Action<GreetingOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
```

**Pattern Mapping:**
- **[Feature]Builder** → `GreetingBuilder`
- **Add[Lifetime]** → `AddSingleton` (chosen for this use case)
- **Deferred Configuration:** ✅ Same pattern

**Key Design Decisions:**
1. **Singleton Lifetime:** Greeting logic is stateless and can be shared
2. **Dependency Resolution:** Gets `ILogger<GreetingService>` from DI container
3. **Deferred Execution:** Configuration actions stored and executed during service instantiation
4. **Fluent API:** Returns `this` for method chaining

---

### Step 4: Create the Extension Method (`GreetingExtensions.cs`)

**Location:** `1.Domain.Services/Greetings/GreetingExtensions.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Services.Greetings;

public static class GreetingExtensions
{
    public static GreetingBuilder AddGreetingService(this IServiceCollection serviceCollection)
    {
        return new GreetingBuilder(serviceCollection);
    }
}
```

**Pattern Mapping:**
- **[Feature]Extensions** → `GreetingExtensions`
- **Add[Feature]()** → `AddGreetingService()`
- **Entry Point:** ✅ Same pattern

---

### Step 5: Create Configuration Methods (`ConfigureGreetings.cs`)

**Location:** `0.App.AppWorker/Greetings/ConfigureGreetings.cs`

This demonstrates how to configure custom greeting strategies for different entity types.

```csharp
using Models.Employees;
using Models.Employees.Interfaces;
using Services.Greetings;

namespace AppWorker.Greetings;

public static partial class GreetingConfiguration
{
    public static GreetingBuilder ConfigureEmployeeGreetings(this GreetingBuilder builder)
    {
        builder.Configure(options =>
        {
            // Custom greeting for employees
            options.AddGreeter<IEmployee>(employee =>
            {
                return $"Good day, {employee.FirstName} {employee.LastName}! Welcome to the team.";
            });
            
            // Custom greeting for credit cards (just for fun)
            options.AddGreeter<ICreditCard>(card =>
            {
                return $"Processing your {card.Issuer} card ending in {card.Number?.Substring(Math.Max(0, card.Number.Length - 4))}";
            });
        });
        
        return builder;
    }
    
    public static GreetingBuilder ConfigureFormalGreetings(this GreetingBuilder builder)
    {
        builder.Configure(options =>
        {
            options.AddGreeter<string>(name =>
            {
                return $"Dear {name}, it is a pleasure to make your acquaintance.";
            });
        });
        
        return builder;
    }
}
```

**Pattern Elements:**
1. **Extension Methods on Builder:** Enable fluent chaining
2. **Domain-Specific Configuration:** Group related greeting strategies
3. **Lambda Expressions:** Concise strategy definitions
4. **Return Builder:** Enable continued chaining

**Comparison to Other Services:**
- **SeederService:** `MockMusic()`, `MockEmployee()`, etc.
- **EncryptionService:** `ObfuscateEmployee()`, etc.
- **GreetingService:** `ConfigureEmployeeGreetings()`, `ConfigureFormalGreetings()`

---

### Step 6: Create Demo Worker (`UsingGreetings.cs`)

**Location:** `0.App.AppWorker/Workers/UsingGreetings.cs`

```csharp
using Microsoft.Extensions.Logging;
using Services.Greetings;
using Services.Seeder;
using Models.Employees.Interfaces;

namespace AppWorker.Workers;

public class UsingGreetings
{
    private readonly ILogger<UsingGreetings> _logger;
    private readonly GreetingService _greetingService;
    private readonly SeederService _seederService;

    public UsingGreetings(ILogger<UsingGreetings> logger, 
        GreetingService greetingService,
        SeederService seederService)
    {
        _logger = logger;
        _greetingService = greetingService;
        _seederService = seederService;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("\n--- Demonstrating GreetingService ---");
        
        // Test greeting with employees
        var employees = _seederService.MockMany<IEmployee>(3).ToList();
        
        _logger.LogInformation("\nGreeting employees:");
        foreach (var employee in employees)
        {
            var greeting = _greetingService.Greet(employee);
            _logger.LogInformation(greeting);
        }
        
        // Test greeting many
        _logger.LogInformation("\nGreeting all employees at once:");
        var greetings = _greetingService.GreetMany(employees);
        foreach (var greeting in greetings)
        {
            _logger.LogInformation(greeting);
        }
        
        // Test default greeting (no custom greeter registered)
        _logger.LogInformation("\nDefault greeting for object without custom greeter:");
        var defaultGreeting = _greetingService.Greet(new { Name = "Unknown Entity" });
        _logger.LogInformation(defaultGreeting);
        
        await Task.CompletedTask;
    }
}
```

**Demonstrates:**
1. **DI Injection:** Service injected via constructor
2. **Single Operation:** `Greet<T>()` for individual entities
3. **Batch Operation:** `GreetMany<T>()` for collections
4. **Default Behavior:** Graceful fallback when no custom greeter exists
5. **Service Composition:** Uses SeederService to generate test data

---

### Step 7: Register Service in DI Container (`Program.cs`)

**Location:** `0.App.AppWorker/Program.cs`

```csharp
using AppWorker.Greetings;
using Services.Greetings;

// ... other registrations ...

// Register GreetingService with configurations
builder.Services
    .AddGreetingService()
    .ConfigureEmployeeGreetings()
    .ConfigureFormalGreetings();

// Register demo worker
builder.Services.AddTransient<UsingGreetings>();
```

**Fluent Configuration Chain:**
1. `AddGreetingService()` → Creates builder, registers service
2. `ConfigureEmployeeGreetings()` → Adds employee-specific greetings
3. `ConfigureFormalGreetings()` → Adds formal string greetings

---

## Pattern Application Checklist

Use this checklist when implementing a new service following this pattern:

### Planning Phase
- [ ] **Identify the domain problem** - What concern needs to be lifted from models?
- [ ] **Define the registry type** - What is the key? What is the value function signature?
- [ ] **Choose DI lifetime** - Singleton, Transient, or Scoped?
- [ ] **List dependencies** - What external services does your service need?
- [ ] **Design operation signatures** - What methods will your service expose?

### Implementation Phase

#### Component 1: Service
- [ ] Create `[Feature]Service` class
- [ ] Add `internal Dictionary<Type, Func<...>>` for registry
- [ ] Inject dependencies via constructor
- [ ] Implement core operation method (e.g., `Execute<T>()`)
- [ ] Implement batch operation method (e.g., `ExecuteMany<T>()`)
- [ ] Add error handling for missing strategies

#### Component 2: Options
- [ ] Create `[Feature]Options` class
- [ ] Take service as constructor parameter
- [ ] Implement `AddStrategy<T>()` method (or domain-specific name like `AddGreeter`)
- [ ] Add type casting wrapper for dictionary storage
- [ ] Apply generic constraints (`where T : class`)

#### Component 3: Builder
- [ ] Create `[Feature]Builder` class
- [ ] Store `IServiceCollection` reference
- [ ] Create `List<Action<[Feature]Options>>` for deferred config
- [ ] In constructor, register service with chosen DI lifetime
- [ ] Use factory pattern `sp => ...` for service instantiation
- [ ] Resolve dependencies from `sp.GetRequiredService<>()`
- [ ] Apply configuration actions during instantiation
- [ ] Implement `Configure()` method returning `this`

#### Component 4: Extensions
- [ ] Create `[Feature]Extensions` static class
- [ ] Implement `Add[Feature]()` extension method
- [ ] Return `[Feature]Builder` instance

#### Component 5: Configuration
- [ ] Create domain-specific configuration extension methods
- [ ] Group related strategies together
- [ ] Use descriptive method names (e.g., `ConfigureEmployeeGreetings`)
- [ ] Return builder for chaining
- [ ] Place in application layer (`0.App.AppWorker/[Feature]/`)

#### Component 6: Usage
- [ ] Create demo worker or controller
- [ ] Inject service via DI
- [ ] Demonstrate single operation
- [ ] Demonstrate batch operation
- [ ] Show error/default handling

#### Component 7: Registration
- [ ] Update `Program.cs` with fluent configuration
- [ ] Chain configuration methods
- [ ] Register demo worker/controller

### Validation Phase
- [ ] Compile without errors
- [ ] Test single operation
- [ ] Test batch operation
- [ ] Test default/error behavior
- [ ] Verify DI resolution
- [ ] Check for memory leaks (if applicable)
- [ ] Review code for SOLID principles

---

## Comparison: Three Service Implementations

| Aspect | SeederService | EncryptionService | GreetingService |
|--------|---------------|-------------------|-----------------|
| **Purpose** | Generate mock data | Encrypt/obfuscate sensitive data | Generate greeting messages |
| **Domain** | Testing | Security | Presentation |
| **Registry Key** | `Type` | `Type` | `Type` |
| **Registry Value** | `Func<SeedGenerator, object>` | `Func<EncryptionService, object, object>` | `Func<object, string>` |
| **Core Method** | `Mock<T>()` | `EncryptAndObfuscate<T>()` | `Greet<T>()` |
| **Batch Method** | `MockMany<T>(count)` | `EncryptAndObfuscateMany<T>(sources)` | `GreetMany<T>(entities)` |
| **DI Lifetime** | Singleton | Transient | Singleton |
| **Dependencies** | None (internal SeedGenerator) | EncryptionEngine | ILogger<GreetingService> |
| **Service Composition** | ❌ No | ✅ Yes (pass service to obfuscator) | ❌ No |
| **Return Type** | `T` | `(T obfuscated, string token)` | `string` |
| **Complexity** | Medium | High | Low |
| **Educational Value** | Introduction to pattern | Advanced composition | Simplicity demonstration |

**Key Insight:** The same architectural pattern successfully applies to three completely different domains, proving its reusability and flexibility.

---

## When to Use This Pattern

### ✅ Good Use Cases
- **Strategy Pattern needs** - Multiple implementations for same interface
- **Type-based dispatch** - Different behavior per entity type
- **Configurable behavior** - Strategies configured at startup
- **Cross-cutting concerns** - Logging, validation, transformation
- **Fluent configuration** - Chainable, readable setup
- **Domain separation** - Lift infrastructure from models

### ❌ Not Ideal For
- **Simple one-off operations** - Overhead not justified
- **No type variation** - Single strategy for all types
- **Dynamic runtime changes** - Strategies fixed at startup
- **Performance-critical paths** - Dictionary lookup overhead
- **No DI container** - Pattern assumes ASP.NET Core DI

---

## Advanced Variations

### Variation 1: Multiple Registry Types
```csharp
internal readonly Dictionary<Type, Func<T, string>> _formatters = new();
internal readonly Dictionary<Type, Func<T, bool>> _validators = new();
```

### Variation 2: Async Operations
```csharp
public async Task<T> ExecuteAsync<T>(T source) where T : class
{
    if (_typeStrategies.TryGetValue(typeof(T), out var strategyFunc))
    {
        return await Task.FromResult((T)strategyFunc(source));
    }
    throw new KeyNotFoundException();
}
```

### Variation 3: Composite Strategies
```csharp
public void AddCompositeStrategy<T>(params Func<T, T>[] strategies)
{
    _typeStrategies[typeof(T)] = (source) =>
    {
        var result = (T)source;
        foreach (var strategy in strategies)
        {
            result = strategy(result);
        }
        return result;
    };
}
```

---

## Further Reading

- [EncryptionVsSeederComparison.md](EncryptionVsSeederComparison.md) - Detailed comparison of Seeder and Encryption implementations
- [SeederServiceArchitecture.md](SeederServiceArchitecture.md) - Deep dive into Seeder architecture
- [EncryptionServiceAssignment.md](EncryptionServiceAssignment.md) - Student assignment for Encryption service
- [Builder Pattern](https://refactoring.guru/design-patterns/builder) - Classic design pattern reference
- [Options Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options) - ASP.NET Core options pattern

---

## Conclusion

The **Pattern Template for Future Services** demonstrates:

1. **Architectural Consistency** - Same four-component structure across different domains
2. **Reusability** - Template applied to Seeder, Encryption, and Greeting services
3. **Scalability** - Easy to add new strategies without modifying core code
4. **Maintainability** - Clear separation of concerns, testable components
5. **Flexibility** - Supports both simple (Greeting) and complex (Encryption) use cases

By following this template, developers can create consistent, well-architected services that integrate seamlessly with ASP.NET Core's dependency injection and follow SOLID principles.
