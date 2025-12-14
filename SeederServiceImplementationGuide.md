# Seeder Service Implementation Guide
## Step-by-Step Transformation from 0-starting-point to Seeder Service Architecture

This guide provides detailed, incremental steps to transform the legacy seeding approach (where models seed themselves) into the modern Seeder Service Architecture described in `SeederServiceArchitecture.md`.

---

## Prerequisites

- Branch: `0-starting-point` checked out
- Understand the problems with the old architecture (models implementing `ISeed<T>`)
- Familiarity with C# functional programming concepts (lambdas, higher-order functions)
- Read `SeederServiceArchitecture.md` for architectural overview

---

## Overview of the Transformation

**Current State (0-starting-point):**
- Models implement `ISeed<T>` interface
- Seeding logic embedded in domain models
- Tight coupling between models and `SeedGenerator`

**Target State (1-seeder-service-architecture):**
- Clean POCO models with no infrastructure dependencies
- Seeding logic in dedicated service layer
- Fluent configuration API using Builder and Options patterns
- Functional programming approach

---

## Step 1: Create the SeederService Core Class

**Goal:** Create the central service that will manage all mockers.

**Location:** `1.Domain.Services/Seeder/SeederService.cs`

```csharp
using Seido.Utilities.SeedGenerator;

namespace Services.Seeder;

public class SeederService
{
    private readonly SeedGenerator _seeder = new SeedGenerator();
    internal readonly Dictionary<Type, Func<SeedGenerator, object>> _typeMockers = 
        new Dictionary<Type, Func<SeedGenerator, object>>();

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
            return Enumerable.Repeat(0, nrInstances)
                .Select(_ => (TInterface)mockerFunc(_seeder));
        }
        throw new KeyNotFoundException($"No mocker found for type {typeof(TInterface).FullName}");
    }
}
```

**Key Concepts:**
- **Registry Pattern**: `_typeMockers` dictionary stores Type → Function mappings
- **Generic Methods**: `Mock<T>()` provides type-safe retrieval
- **Higher-Order Functions**: Stores functions (`Func<>`) as data
- **LINQ**: `MockMany()` uses `Repeat` and `Select` for functional generation

**Test This Step:**
```csharp
// In a test or console app
var service = new SeederService();
// Won't work yet - no mockers registered!
// But the class compiles and structure is in place
```

---

## Step 2: Create the SeederOptions Configuration Class

**Goal:** Provide a type-safe API for registering mockers.

**Location:** `1.Domain.Services/Seeder/SeederOptions.cs`

```csharp
using Seido.Utilities.SeedGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

public class SeederOptions
{
    private readonly SeederService _seedService;

    public SeederOptions(SeederService seedService)
    {
        _seedService = seedService;
    }

    // For interface/implementation pairs
    public void AddMocker<TInterface, TInstance>(
        Func<SeedGenerator, TInstance, TInstance> mocker)
        where TInterface : class
        where TInstance : new()
    {
        if (!typeof(TInterface).IsInterface)
            throw new ArgumentException($"Type {typeof(TInterface).Name} must be an interface");

        _seedService._typeMockers[typeof(TInterface)] = 
            (seeder) => mocker(seeder, new TInstance());
    }
    
    // For concrete classes
    public void AddMocker<TInstance>(
        Func<SeedGenerator, TInstance, TInstance> mocker)
        where TInstance : new()
    {
        if (!typeof(TInstance).IsClass)
            throw new ArgumentException($"Type {typeof(TInstance).Name} must be a class");

        _seedService._typeMockers[typeof(TInstance)] = 
            (seeder) => mocker(seeder, new TInstance());
    }
}
```

**Key Concepts:**
- **Options Pattern**: Strongly-typed configuration API
- **Generic Constraints**: Ensures type safety at compile time
- **Partial Application**: Transforms `Func<SeedGenerator, TInstance, TInstance>` into `Func<SeedGenerator, object>`
- **Validation**: Runtime checks for interface/class requirements

**Test This Step:**
```csharp
// Manual test without DI
var service = new SeederService();
var options = new SeederOptions(service);

options.AddMocker<FamousQuote>((seeder, quote) =>
{
    quote.Quote = "Test Quote";
    return quote;
});

var result = service.Mock<FamousQuote>();
Console.WriteLine(result.Quote); // Output: Test Quote
```

---

## Step 3: Create the SeederBuilder Class

**Goal:** Implement the Builder pattern for fluent configuration with deferred execution.

**Location:** `1.Domain.Services/Seeder/SeederBuilder.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

public class SeederBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Action<SeederOptions>> _configureActions = 
        new List<Action<SeederOptions>>();

    public SeederBuilder(IServiceCollection services)
    {
        _services = services;
        
        // Register the SeederService with deferred configuration
        _services.AddSingleton<SeederService>(sp =>
        {
            var seedService = new SeederService();
            if (_configureActions.Any())
            {
                var options = new SeederOptions(seedService);
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }
            return seedService;
        });
    }

    public SeederBuilder Configure(Action<SeederOptions> configure)
    {
        _configureActions.Add(configure);
        return this;
    }
}
```

**Key Concepts:**
- **Builder Pattern**: Step-by-step construction with method chaining
- **Deferred Execution**: Configuration actions stored, executed later
- **Factory Pattern**: Service created via factory function in DI
- **Fluent Interface**: Returns `this` for chaining

**Why Deferred Configuration?**
Configuration needs to happen AFTER all `Configure()` calls are made but BEFORE the service is used. The factory pattern in DI registration ensures this timing.

---

## Step 4: Create the Extension Method Entry Point

**Goal:** Provide a conventional .NET extension method for registration.

**Location:** `1.Domain.Services/Seeder/SeederExtensions.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Services.Seeder;

public static class SeederExtensions
{
    public static SeederBuilder AddSeeder(this IServiceCollection serviceCollection)
    {
        return new SeederBuilder(serviceCollection);
    }
}
```

**Key Concepts:**
- **Extension Method**: Non-invasive extension of `IServiceCollection`
- **Convention**: Follows .NET patterns like `AddDbContext()`, `AddLogging()`
- **Entry Point**: Starting point for fluent configuration chain

**Test This Step:**
```csharp
// In Program.cs (won't fully work yet - no mockers configured)
using Services.Seeder;

builder.Services.AddSeeder(); // Returns SeederBuilder
```

---

## Step 5: Clean Up Domain Models

**Goal:** Remove seeding logic from domain models to make them pure POCOs.

**Location:** `2.Domain.Models/FamousQuote.cs`

**Before:**
```csharp
using Seido.Utilities.SeedGenerator;

namespace Models
{
    public class FamousQuote : ISeed<FamousQuote>
    {
        public Guid QuoteId {get; set;} = Guid.NewGuid();
        public string Quote { get; set; }
        public string Author { get; set; }

        #region randomly seed this instance
        public virtual bool Seeded { get; set; } = false;
        public virtual FamousQuote Seed(SeedGenerator seedGenerator)
        {
            Seeded = true;
            QuoteId = Guid.NewGuid();
        
            var q = seedGenerator.Quote;
            Author = q.Author;
            Quote = q.Quote;
            return this;
        }
        #endregion
    }
}
```

**After:**
```csharp
namespace Models
{
    // Clean POCO - no infrastructure dependencies!
    public class FamousQuote
    {
        public Guid QuoteId {get; set;} = Guid.NewGuid();
        public string Quote { get; set; }
        public string Author { get; set; }

        public FamousQuote() {}
        public FamousQuote(FamousQuote original)
        {
            QuoteId = original.QuoteId;
            Quote = original.Quote;
            Author = original.Author;
        }
    }
}
```

**Changes:**
1. ❌ Remove `ISeed<FamousQuote>` interface
2. ❌ Remove `Seeded` property
3. ❌ Remove `Seed()` method
4. ❌ Remove `using Seido.Utilities.SeedGenerator;`
5. ✅ Keep copy constructor for cloning

**Repeat for:**
- `2.Domain.Models/Latin.cs` → `LatinSentence` class
- Any other models implementing `ISeed<T>`

---

## Step 6: Create Mock Configuration Extensions

**Goal:** Create extension methods that configure mockers for each domain.

**Location:** `0.App.AppWorker/Mocking/MockQuote.cs`

```csharp
using Models;
using Seido.Utilities.SeedGenerator;
using Services.Seeder;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockQuote(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<FamousQuote>((seeder, quote) =>
            {
                quote.QuoteId = Guid.NewGuid();
             
                var q = seeder.Quote;
                quote.Author = q.Author;
                quote.Quote = q.Quote;
                return quote;
            });
        });
        return seedBuilder;
    }
}
```

**Key Concepts:**
- **Extension Method on Builder**: Extends `SeederBuilder` for domain-specific config
- **Partial Class**: Multiple files contribute to same static class
- **Lambda Expression**: Inline function defines mocking logic
- **Closure**: Lambda can capture external variables if needed
- **Method Chaining**: Returns builder for fluent syntax

**Location:** `0.App.AppWorker/Mocking/MockLatin.cs`

```csharp
using Models;
using Services.Seeder;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockLatin(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<LatinSentence>((seeder, latin) =>
            {
                latin.SentenceId = Guid.NewGuid();
                latin.Sentence = seeder.LatinSentence;
                latin.Paragraph = seeder.LatinParagraph;
                return latin;
            });
        });
        return seedBuilder;
    }
}
```

**Location:** `0.App.AppWorker/Mocking/MockMusic.cs`

```csharp
using Models;
using Models.Music.Interfaces;
using Models.Music;
using Seido.Utilities.SeedGenerator;
using Services.Seeder;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockMusic(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            // Interface + Implementation pattern for dependency inversion
            options.AddMocker<IArtist, Artist>((seeder, artist) =>
            {
                artist.ArtistId = Guid.NewGuid();
                artist.FirstName = seeder.FirstName;
                artist.LastName = seeder.LastName;
                artist.BirthDay = seeder.Bool ? seeder.DateAndTime(1940, 1990) : null;
                return artist;
            });

            options.AddMocker<IAlbum, Album>((seeder, album) =>
            {
                album.AlbumId = Guid.NewGuid();
                album.Name = seeder.MusicAlbumName;
                album.CopiesSold = seeder.Next(1_000, 1_000_000);
                album.ReleaseYear = seeder.Next(1970, 2024);
                return album;       
            });

            options.AddMocker<IMusicGroup, MusicGroup>((seeder, musicGroup) =>
            {
                musicGroup.MusicGroupId = Guid.NewGuid();
                musicGroup.Name = seeder.MusicGroupName;
                musicGroup.EstablishedYear = seeder.Next(1970, 2024);
                musicGroup.Genre = seeder.FromEnum<MusicGenre>();
                return musicGroup;
            });
        });
        return seedBuilder;
    }
}
```

**Why Partial Classes?**
- **Organization**: One file per domain concept (Quote, Latin, Music)
- **Maintainability**: Easy to find and update specific mockers
- **Extensibility**: New domains can add new files without touching existing code
- **Grouping**: All extensions share the same class name and namespace

---

## Step 7: Configure in Program.cs

**Goal:** Wire everything together using the fluent API.

**Location:** `0.App.AppWorker/Program.cs`

**Before:**
```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Other services...
builder.Services.AddTransient<UsingSeeder>();

var host = builder.Build();
host.Run();
```

**After:**
```csharp
using AppWorker.Mocking;
using Services.Seeder;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Configure Seeder Service with fluent API
builder.Services.AddSeeder()
    .MockMusic()
    .MockLatin()
    .MockQuote();

// Other services...
builder.Services.AddTransient<UsingSeeder>();

var host = builder.Build();
host.Run();
```

**Key Concepts:**
- **Fluent API**: Chained method calls read like a sentence
- **DSL-like Syntax**: Configuration reads like a domain-specific language
- **Order Independence**: Mocker order doesn't matter (all registered before service creation)

---

## Step 8: Update Usage Code

**Goal:** Update code that previously used `ISeed<T>` to use `SeederService`.

**Location:** `0.App.AppWorker/Workers/UsingSeeder.cs`

**Before:**
```csharp
public class UsingSeeder
{
    private readonly ILogger<UsingSeeder> _logger;

    public async Task ExecuteAsync()
    {
        var seeder = new SeedGenerator();
        
        // Old way - model seeds itself
        var quote = new FamousQuote().Seed(seeder);
        var quotes = Enumerable.Range(0, 10)
            .Select(_ => new FamousQuote().Seed(seeder))
            .ToList();
    }
}
```

**After:**
```csharp
public class UsingSeeder
{
    private readonly ILogger<UsingSeeder> _logger;
    private readonly SeederService _seederService;

    public UsingSeeder(ILogger<UsingSeeder> logger, SeederService seederService)
    {
        _logger = logger;
        _seederService = seederService;
    }

    public async Task ExecuteAsync()
    {
        // New way - service creates mocked instances
        var quote = _seederService.Mock<FamousQuote>();
        var quotes = _seederService.MockMany<FamousQuote>(10);
        
        // Works with interfaces too!
        var artist = _seederService.Mock<IArtist>();
        var artists = _seederService.MockMany<IArtist>(5);
    }
}
```

**Changes:**
1. ✅ Inject `SeederService` via constructor
2. ✅ Use `Mock<T>()` for single instances
3. ✅ Use `MockMany<T>(count)` for collections
4. ❌ Remove direct `SeedGenerator` usage
5. ❌ Remove `.Seed()` method calls

---

## Step 9: Test the Complete Implementation

**Goal:** Verify everything works end-to-end.

**Test 1: Basic Mocking**
```csharp
// In UsingSeeder or test class
public async Task TestBasicMocking()
{
    var quote = _seederService.Mock<FamousQuote>();
    _logger.LogInformation($"Quote: {quote.Quote} - {quote.Author}");
    
    var latin = _seederService.Mock<LatinSentence>();
    _logger.LogInformation($"Latin: {latin.Sentence}");
}
```

**Test 2: Collection Generation**
```csharp
public async Task TestCollectionGeneration()
{
    var quotes = _seederService.MockMany<FamousQuote>(10);
    _logger.LogInformation($"Generated {quotes.Count()} quotes");
    
    foreach (var quote in quotes)
    {
        _logger.LogInformation($"  - {quote.Quote}");
    }
}
```

**Test 3: Interface-Based Mocking**
```csharp
public async Task TestInterfaceMocking()
{
    var artist = _seederService.Mock<IArtist>();
    _logger.LogInformation($"Artist: {artist.FirstName} {artist.LastName}");
    
    var artists = _seederService.MockMany<IArtist>(5)
        .OrderBy(a => a.LastName);
        
    foreach (var a in artists)
    {
        _logger.LogInformation($"  - {a.FullName}");
    }
}
```

**Expected Results:**
- ✅ All quotes have random data from seed generator
- ✅ Latin sentences are populated
- ✅ Artists have names and optional birthdates
- ✅ No null reference exceptions
- ✅ Data is different each run (random)

---

## Step 10: Advanced Configurations (Optional)

### Environment-Specific Mocking

```csharp
// In Program.cs
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSeeder()
        .MockMusic()
        .MockLatin()
        .MockQuote();
}
else if (builder.Environment.IsStaging())
{
    builder.Services.AddSeeder()
        .MockMusic()
        .MockLatin();
        // No quotes in staging
}
```

### Custom Mocker Logic

```csharp
// In Mocking/MockCustom.cs
public static partial class SeederMocking
{
    public static SeederBuilder MockCustomEntity(this SeederBuilder builder)
    {
        var creationTime = DateTime.UtcNow;
        
        return builder.Configure(options =>
        {
            options.AddMocker<CustomEntity>((seeder, entity) =>
            {
                entity.Id = Guid.NewGuid();
                entity.Name = seeder.FirstName;
                entity.CreatedAt = creationTime; // Captured from closure!
                entity.IsActive = seeder.Bool;
                return entity;
            });
        });
    }
}
```

### Conditional Mocking

```csharp
public static SeederBuilder MockConditional(this SeederBuilder builder)
{
    return builder.Configure(options =>
    {
        options.AddMocker<Product>((seeder, product) =>
        {
            product.Id = Guid.NewGuid();
            product.Name = seeder.MusicAlbumName;
            
            // Conditional logic
            if (seeder.Bool)
            {
                product.Category = "Electronics";
                product.Price = seeder.NextDecimal(100, 1000);
            }
            else
            {
                product.Category = "Books";
                product.Price = seeder.NextDecimal(10, 50);
            }
            
            return product;
        });
    });
}
```

---

## Troubleshooting Guide

### Issue: "No mocker found for type X"

**Cause:** Mocker not registered for that type.

**Solution:**
```csharp
// Add mocker in extension method
public static SeederBuilder MockX(this SeederBuilder builder)
{
    return builder.Configure(options =>
    {
        options.AddMocker<X>((seeder, x) => { /* configure */ return x; });
    });
}

// Register in Program.cs
builder.Services.AddSeeder().MockX();
```

### Issue: "Type must be an interface"

**Cause:** Using interface/implementation overload with concrete class.

**Solution:**
```csharp
// Wrong
options.AddMocker<FamousQuote, FamousQuote>(...); // Both are classes!

// Correct
options.AddMocker<FamousQuote>(...); // Single-parameter overload
```

### Issue: "Type must have a parameterless constructor"

**Cause:** Type doesn't have `new()` constraint compatible constructor.

**Solution:**
```csharp
// Add parameterless constructor to your model
public class MyModel
{
    public MyModel() { } // Add this
    public MyModel(string param) { } // Parameterized still allowed
}
```

### Issue: Service not injected / NullReferenceException

**Cause:** Forgot to register seeder in Program.cs.

**Solution:**
```csharp
// Ensure this line exists in Program.cs
builder.Services.AddSeeder().MockMusic().MockLatin().MockQuote();
```

### Issue: Mockers not executing

**Cause:** Configuration called after service resolution.

**Solution:** Ensure all `.MockX()` calls happen during startup, before `builder.Build()`.

---

## Verification Checklist

Before considering the migration complete, verify:

- [ ] All models are clean POCOs (no `ISeed<T>`)
- [ ] `SeederService.cs` exists and compiles
- [ ] `SeederOptions.cs` exists and compiles  
- [ ] `SeederBuilder.cs` exists and compiles
- [ ] `SeederExtensions.cs` exists and compiles
- [ ] Mock extension methods created for all domains
- [ ] `Program.cs` registers seeder with fluent API
- [ ] All usage code updated to inject `SeederService`
- [ ] Application runs without errors
- [ ] Mock data is generated correctly
- [ ] Tests pass (if applicable)

---

## Summary of Benefits Achieved

### Before (0-starting-point)
```csharp
// ❌ Model polluted with infrastructure
public class FamousQuote : ISeed<FamousQuote>
{
    public FamousQuote Seed(SeedGenerator gen) { ... }
}

// ❌ Direct coupling
var quote = new FamousQuote().Seed(seeder);
```

### After (1-seeder-service-architecture)
```csharp
// ✅ Clean domain model
public class FamousQuote
{
    public string Quote { get; set; }
}

// ✅ Service-based mocking
var quote = _seederService.Mock<FamousQuote>();

// ✅ Fluent configuration
services.AddSeeder()
    .MockMusic()
    .MockLatin()
    .MockQuote();
```

### Architecture Improvements
1. **Separation of Concerns** - Infrastructure separate from domain
2. **Testability** - Easy to mock and test services
3. **Flexibility** - Environment-specific configurations
4. **Maintainability** - Changes localized to service layer
5. **Extensibility** - New mockers added without modifying models
6. **SOLID Principles** - SRP, DIP, OCP all satisfied
7. **Functional Programming** - Leverages lambdas, higher-order functions, composition

---

## Next Steps

1. **Add More Mockers**: Create mock extensions for additional domain models
2. **Custom Seed Data**: Configure different seed data for different scenarios
3. **Integration Tests**: Write tests that verify seeder service behavior
4. **Documentation**: Document your custom mockers and their behavior
5. **Performance**: Profile and optimize if generating large datasets

---

## Related Documentation

- [SeederServiceArchitecture.md](./SeederServiceArchitecture.md) - Architectural overview
- [PokerGameAssignment.md](./PokerGameAssignment.md) - Example of functional programming patterns

---

**Congratulations!** You've successfully transformed from a tightly-coupled, model-based seeding approach to a flexible, functional, service-based architecture. 🎉
