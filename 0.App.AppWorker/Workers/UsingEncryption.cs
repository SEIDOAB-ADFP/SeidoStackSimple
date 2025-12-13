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



        // _logger.LogInformation("Mocked MusicGroup: {MusicGroupName}", mockMusicGroup.Name);
        // _logger.LogInformation("Mocked Artist: {FirstName}", mockArtist.FirstName);
        // _logger.LogInformation("Mocked Album: {AlbumName}", mockAlbum.Name);

        // _logger.LogInformation("Mocked Album: {Artists}", string.Join("\r\n", mockArtists.Select(a => a.FirstName).ToList()));
        // _logger.LogInformation("Mocked Latin Sentences: {Sentences}", string.Join("\r\n", latins.Select(l => l.Sentence).ToList()));
        // _logger.LogInformation("Mocked Quotes: {Quotes}", string.Join("\r\n", quotes.Select(q => q.Quote).ToList()));

        await Task.CompletedTask;
    }
}
