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
