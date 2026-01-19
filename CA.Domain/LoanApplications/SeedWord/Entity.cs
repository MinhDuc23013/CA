namespace CA.Domain.LoanApplications.SeedWord
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;

        protected Entity() { }
    }
}
