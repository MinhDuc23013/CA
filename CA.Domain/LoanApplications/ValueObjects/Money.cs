using CA.Domain.LoanApplications.Exceptions;

namespace CA.Domain.LoanApplications.ValueObjects
{
    public sealed record Money(decimal Amount, string Currency)
    {
        public static Money Zero(string currency) => new(0, currency);

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new DomainException("Currency mismatch");

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static bool operator >(Money a, Money b)
            => a.Currency == b.Currency && a.Amount > b.Amount;

        public static bool operator <(Money a, Money b)
            => a.Currency == b.Currency && a.Amount < b.Amount;
    }

}
