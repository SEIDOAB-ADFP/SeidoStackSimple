using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Services.Encryptions;
using Models.Employees;
using Models.Employees.Interfaces;
using System.Text.RegularExpressions;

namespace AppWorker.Obfuscation;

public static partial class EncryptionObfuscation
{
    public static EncryptionBuilder ObfuscateEmployee(this EncryptionBuilder builder)
    {       
        builder.Configure(options => { 
            // Configure options if needed
            options.AddObfuscator<ICreditCard, CreditCard>((_, cc) =>
            {
                //your obfuscation logic here

                return cc;
            });
            options.AddObfuscator<IEmployee, Employee>((encryptionService, emp) =>
            {
                //your obfuscation logic here

                //Example: obfuscate credit cards
                emp.CreditCards = encryptionService.ObfuscateMany<ICreditCard>(emp.CreditCards).ToList<ICreditCard>();
                return emp;
            });
        });
        return builder;
    }
}