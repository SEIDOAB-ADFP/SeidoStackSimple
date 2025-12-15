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

        // Test formal greeting
        _logger.LogInformation("\nFormal greeting for a name:");
        var formalGreeting = _greetingService.Greet("John Doe");
        _logger.LogInformation(formalGreeting);

        // Test default greeting (no custom greeter registered)
        _logger.LogInformation("\nDefault greeting for object without custom greeter:");
        var defaultGreeting = _greetingService.Greet(new { Name = "Unknown Entity" });
        _logger.LogInformation(defaultGreeting);
        
        await Task.CompletedTask;
    }
}
