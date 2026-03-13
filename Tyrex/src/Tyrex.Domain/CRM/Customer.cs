using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.CRM;

public sealed class Customer : AggregateRoot, IAuditableEntity
{
    private Customer(Guid id, string firstName, string lastName, string email, string phone, CustomerType type, string? companyName)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Type = type;
        CompanyName = companyName;
    }

    private Customer()
    {
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public CustomerType Type { get; private set; }
    public string? CompanyName { get; private set; }
    
    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static Customer CreateIndividual(string firstName, string lastName, string email, string phone)
    {
        return new Customer(Guid.NewGuid(), firstName, lastName, email, phone, CustomerType.Individual, null);
    }

    public static Customer CreateCompany(string companyName, string contactFirstName, string contactLastName, string email, string phone)
    {
        return new Customer(Guid.NewGuid(), contactFirstName, contactLastName, email, phone, CustomerType.Company, companyName);
    }

    public void UpdateContactInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
}

public enum CustomerType
{
    Individual = 1,
    Company = 2
}
