namespace Models.Employees.Interfaces;

public enum WorkRole {AnimalCare, Veterinarian, ProgramCoordinator, Maintenance, Management}

public interface IEmployee
{
    public Guid EmployeeId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }
    public WorkRole Role { get; set; }

    public List<ICreditCard> CreditCards { get; set; }
}

public enum CardIssuer {AmericanExpress, Visa, MasterCard, DinersClub}

public interface ICreditCard
{
    public Guid CreditCardId { get; set; }

    public CardIssuer Issuer { get; set; }
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }
    public string EnryptedToken { get; set; }
}