using Configuration;
using Security;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Employees;
using Models.Employees.Interfaces;
using Seido.Utilities.SeedGenerator;
namespace Services.Employees;


public class EmployeeService : IEmployeeService {
    private readonly ILogger<EmployeeService> _logger;
    private readonly EncryptionEngine _encryptions;
    private readonly List<IEmployee> _employees;
    private readonly SeedGenerator _seeder = new SeedGenerator();

    public EmployeeService(ILogger<EmployeeService> logger, EncryptionEngine encryptions)
    {
        _logger = logger;
        _encryptions = encryptions;

        _logger.LogInformation("Randomly generating 1000 employees");
        var employees = _seeder.ItemsToList<Employee>(1000);

        foreach (var emp in employees)
        {
            var cc = _seeder.ItemsToList<CreditCard>(_seeder.Next(0, 4));
            foreach (var c in cc)
            {
                emp.CreditCards.Add(c.EnryptAndObfuscate(_encryptions.AesEncryptToBase64<CreditCard>));
            }
        }
    
        _employees = employees.ToList<IEmployee>();
    }   

    public ResponsePageDto<IEmployee> ReadEmployees(int pageNumber, int pageSize)
    {
        _logger.LogInformation($"Retrieving {pageSize} employees from page {pageNumber}");

        var emp =_employees.Skip(pageNumber * pageSize).Take(pageSize).Select(e => new Employee(e)).ToList<IEmployee>();
        emp.ForEach(e => e.CreditCards.ForEach(cc => cc.EnryptedToken = null));

        var ret = new ResponsePageDto<IEmployee>()
        { 
            DbItemsCount = _employees.Count,
            PageNr = pageNumber,
            PageSize = pageSize,
            PageItems = emp
        };        

        return ret;
    }

    public IEmployee ReadEmployee(Guid id, bool cc_decrypted)
    {
        IEmployee item = _employees.FirstOrDefault(e => e.EmployeeId == id);
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        if (cc_decrypted)
        {
            List<ICreditCard> decryptedCards = new List<ICreditCard>();
            foreach (var cc in item.CreditCards)
            {
                var decrypted = new CreditCard()
                    .Decrypt(_encryptions.AesDecryptFromBase64<CreditCard>, cc.EnryptedToken);
                decryptedCards.Add(decrypted);
            }
            item.CreditCards = decryptedCards;
        }
        return item;
    }
}