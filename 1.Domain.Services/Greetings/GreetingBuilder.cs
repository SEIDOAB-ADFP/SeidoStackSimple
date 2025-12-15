using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
