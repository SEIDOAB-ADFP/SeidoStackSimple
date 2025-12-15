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
