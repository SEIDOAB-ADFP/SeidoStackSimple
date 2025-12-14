using Models;
using Models.Employees;
using Models.Employees.Interfaces;
using Newtonsoft.Json;
using Seido.Utilities.SeedGenerator;
using Services.Employees;


namespace AppWorker.Workers;

public class UsingEncryption
{
    private readonly ILogger<UsingEncryption> _logger;
    private readonly IEmployeeService _employeeService;

    public UsingEncryption(ILogger<UsingEncryption> logger, IEmployeeService employeeService)
    {
        _logger = logger;
        _employeeService = employeeService;
    }

    public async Task ExecuteAsync()
    {

        var employeesPage = _employeeService.ReadEmployees(0, 100);

        var emp = employeesPage.PageItems.First(e => e.CreditCards.Count > 0);

        _logger.LogInformation("An employee with encrypted credit cards: {EmployeeDetails}\n", 
            JsonConvert.SerializeObject(emp, Formatting.Indented));

        emp = _employeeService.ReadEmployee(emp.EmployeeId, true);

        _logger.LogInformation("Same employee with decrypted credit cards: {EmployeeDetails}\n", 
            JsonConvert.SerializeObject(emp, Formatting.Indented));

        await Task.CompletedTask;
    }
}
