using Configuration;
using Security;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Employees;
using Models.Employees.Interfaces;
using Services.Encryptions;
using Services.Seeder;

namespace Services.Employees;


public class EmployeeService : IEmployeeService {
    private readonly ILogger<EmployeeService> _logger;
    private readonly EncryptionEngine _encryptions;
    private readonly SeederService _seederService;
    private readonly EncryptionService _encryptionService;
    private readonly List<IEmployee> _employees;

    public EmployeeService(ILogger<EmployeeService> logger, EncryptionEngine EncryptionEngine,
        SeederService seederService, EncryptionService encryptionService)
    {
        _logger = logger;
        _encryptions = EncryptionEngine;
        _seederService = seederService;
        _encryptionService = encryptionService;
        _logger.LogInformation("Randomly generating 1000 employees");

        //ToList is needed here to avoid deferred execution
        var rnd = new Random();
        var employees = _seederService.MockMany<IEmployee>(1000).ToList();
        foreach (var emp in employees)
        {
            emp.CreditCards = _seederService.MockMany<ICreditCard>(rnd.Next(0, 4)).ToList<ICreditCard>();

            //using the EncryptionService to test the EncryptAndObfuscate and Decrypt methods
            //var encryptEmp = _encryptionService.EncryptAndObfuscate<IEmployee>(emp);
            //var decryptedEmp = _encryptionService.Decrypt<Employee>(encryptEmp.encryptedToken);

            var encryptedCards = new List<ICreditCard>();
            foreach (var c in emp.CreditCards)
            {
                //using the EncryptionService to test the EncryptAndObfuscate and Decrypt methods
                var encryptedCard = _encryptionService.EncryptAndObfuscate<ICreditCard>(c);
                var decryptedCard = _encryptionService.Decrypt<ICreditCard>(encryptedCard.encryptedToken);

                encryptedCard.obfuscatedObject.EnryptedToken = encryptedCard.encryptedToken;
                encryptedCards.Add(encryptedCard.obfuscatedObject);
            }
            emp.CreditCards = encryptedCards;
        }
        
        var i = employees.Count(e => e.CreditCards.Count > 0);
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
                var decrypted = _encryptionService.Decrypt<ICreditCard>(cc.EnryptedToken);
                decryptedCards.Add(decrypted);
            }
            item.CreditCards = decryptedCards;
        }
        return item;
    }
}