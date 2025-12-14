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
                string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
                string replacement = "$1**** **** **** $4";
                cc.Number = Regex.Replace(cc.Number, pattern, replacement);

                cc.ExpirationYear = "**";
                cc.ExpirationMonth = "**";

                return cc;
            });
            options.AddObfuscator<IEmployee, Employee>((encryptionService, emp) =>
            {
                emp.LastName = "***";
                emp.HireDate = default;
                emp.Role = WorkRole.Undefined;

                emp.CreditCards = encryptionService.ObfuscateMany<ICreditCard>(emp.CreditCards).ToList<ICreditCard>();
                return emp;
            });
        });
        return builder;
    }
}