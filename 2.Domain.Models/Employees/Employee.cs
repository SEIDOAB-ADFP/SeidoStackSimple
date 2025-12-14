using Configuration;
using Models.Employees.Interfaces;
using Newtonsoft.Json;

namespace Models.Employees;

public class Employee:IEmployee
{
    public Guid EmployeeId { get; set; }    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public WorkRole Role { get; set; }

    public List<ICreditCard> CreditCards { get; set; } = new List<ICreditCard>();   

    public Employee() {}
    public Employee(IEmployee original)
    {
        EmployeeId = original.EmployeeId;
        FirstName = original.FirstName;
        LastName = original.LastName;
        HireDate = original.HireDate;
        Role = original.Role;
        CreditCards = original.CreditCards.Select(cc => new CreditCard(cc)).ToList<ICreditCard>();
    }
}