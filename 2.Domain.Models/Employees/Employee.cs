using Configuration;
using Models.Employees.Interfaces;
using Newtonsoft.Json;
using Seido.Utilities.SeedGenerator;

namespace Models.Employees;

public class Employee:IEmployee, ISeed<Employee>
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
        CreditCards = original.CreditCards.Select(cc => new CreditCard(cc)).Cast<ICreditCard>().ToList();
    }

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Employee Seed (SeedGenerator seeder)
    {
        Seeded = true;
        EmployeeId = Guid.NewGuid();
        
        FirstName = seeder.FirstName;
        LastName = seeder.LastName;
        HireDate = seeder.DateAndTime(2000, 2024);
        Role = seeder.FromEnum<WorkRole>();

        return this;
    }
    #endregion
    
}