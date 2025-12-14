using Configuration;
using Security;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Employees;
using Models.Employees.Interfaces;
using Newtonsoft.Json;
using Services.Seeder;

namespace Services.Employees;


public class EmployeeService : IEmployeeService {
    private readonly ILogger<EmployeeService> _logger;
    private readonly EncryptionEngine _encryptions;
    private readonly SeederService _seederService;
    private readonly List<IEmployee> _employees;

    public EmployeeService(ILogger<EmployeeService> logger, EncryptionEngine encryptions,
        SeederService seederService)
    {
        _logger = logger;
        _encryptions = encryptions;
        _seederService = seederService;

        _logger.LogInformation("Randomly generating 1000 employees");

        //ToList is needed here to avoid deferred execution
        var rnd = new Random();
        var employees = _seederService.MockMany<IEmployee>(1000).ToList();
        foreach (var emp in employees)
        {
            var cc = _seederService.MockMany<ICreditCard>(rnd.Next(0, 4));
            foreach (var c in cc)
            {
                emp.CreditCards.Add(((CreditCard)c).EnryptAndObfuscate(_encryptions.AesEncryptToBase64<CreditCard>));
            }
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
                var decrypted = new CreditCard()
                    .Decrypt(_encryptions.AesDecryptFromBase64<CreditCard>, cc.EnryptedToken);
                decryptedCards.Add(decrypted);
            }
            item.CreditCards = decryptedCards;
        }
        return item;
    }
}