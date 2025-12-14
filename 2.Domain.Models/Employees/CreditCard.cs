using System.Text.RegularExpressions;
using Configuration;
using Models.Employees.Interfaces;
using Newtonsoft.Json;

namespace Models.Employees;

public class CreditCard : ICreditCard
{
    public Guid CreditCardId { get; set; }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public CardIssuer Issuer { get; set; }
    
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }

    public string EnryptedToken { get; set; }
    

    public CreditCard() {}
    public CreditCard(ICreditCard original)
    {
        CreditCardId = original.CreditCardId;
        Issuer = original.Issuer;
        Number = original.Number;
        ExpirationYear = original.ExpirationYear;
        ExpirationMonth = original.ExpirationMonth;
        EnryptedToken = original.EnryptedToken;
    }
}