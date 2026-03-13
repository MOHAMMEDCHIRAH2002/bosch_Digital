using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Billing;

public sealed class Payment : Entity
{
    internal Payment(Guid id, decimal amount, DateTime paymentDate, PaymentMethod method, string? referenceInfo)
        : base(id)
    {
        Amount = amount;
        PaymentDate = paymentDate;
        Method = method;
        ReferenceInfo = referenceInfo;
    }

    private Payment() { }

    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? ReferenceInfo { get; private set; }
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    BankTransfer = 3,
    Check = 4
}
