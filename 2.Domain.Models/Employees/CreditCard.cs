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

    //obfuscation is when you hide parts of the data, encryption is when you encode the data
    public CreditCard EnryptAndObfuscate(Func<CreditCard, string> encryptor)
    {
        this.EnryptedToken = encryptor(this);

        string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        string replacement = "$1**** **** **** $4";
        this.Number = Regex.Replace(Number, pattern, replacement);

        this.ExpirationYear = "**";
        this.ExpirationMonth = "**";

        return this;
    }

    public ICreditCard Decrypt(Func<string, JsonSerializerSettings, ICreditCard> decryptor, string encryptedToken)
    {
        return decryptor(encryptedToken, new JsonSerializerSettings
        {
            Converters = {
                new Configuration.AbstractConverter<ICreditCard, CreditCard>()}
        });
    }
}