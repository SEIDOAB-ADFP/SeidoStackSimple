using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Services.Seeder;
using Models.Employees;
using Models.Employees.Interfaces;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockEmployee(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<IEmployee, Employee>((seeder, employee) =>
            {
                employee.EmployeeId = Guid.NewGuid();
                
                employee.FirstName = seeder.FirstName;
                employee.LastName = seeder.LastName;
                employee.HireDate = seeder.DateAndTime(2000, 2024);
                employee.Role = seeder.FromEnum<WorkRole>();
                return employee;
            });
            options.AddMocker<ICreditCard, CreditCard>((seeder, cc) =>
            {
                cc.CreditCardId = Guid.NewGuid();
               
                cc.Issuer = seeder.FromEnum<CardIssuer>();
                cc.Number = $"{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}";
                cc.ExpirationYear = $"{seeder.Next(25, 32)}";
                cc.ExpirationMonth = $"{seeder.Next(01, 13):D2}";
                return cc;
            });
        });
        return seedBuilder;
    }
}