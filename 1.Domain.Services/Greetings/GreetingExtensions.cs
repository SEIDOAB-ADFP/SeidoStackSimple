using Microsoft.Extensions.DependencyInjection;

namespace Services.Greetings;

public static class GreetingExtensions
{
    public static GreetingBuilder AddGreetingService(this IServiceCollection serviceCollection)
    {
        return new GreetingBuilder(serviceCollection);
    }
}
