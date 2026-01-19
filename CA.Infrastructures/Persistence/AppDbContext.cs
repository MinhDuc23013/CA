using CA.Domain.LoanApplications.Aggregates;
using CA.Domain.LoanApplications.SeedWord;
using Microsoft.EntityFrameworkCore;

namespace CA.Infrastructures.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        //public DbSet<Disbursement> Disbursements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoanApplication>(entity =>
            {
                entity.ToTable("LoanApplication");
                entity.HasKey(e => e.Id);

                // Configure the backing field "_disbursements" as a collection navigation
                //entity.HasMany<Disbursement>("_disbursements")
                //      .WithOne()
                //      .HasForeignKey("LoanApplicationId")
                //      .OnDelete(DeleteBehavior.Cascade);

                //entity.Navigation("_disbursements")
                //      .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            //modelBuilder.Entity<Disbursement>(entity =>
            //{
            //    entity.ToTable("Disbursement");
            //    entity.HasKey(e => e.Id);
            //});

        }

        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            // 1️⃣ Collect domain events
            var domainEvents = ChangeTracker
                .Entries<AggregateRoot<Guid>>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // 2️⃣ Save DB
            var result = await base.SaveChangesAsync(cancellationToken);

            // 3️⃣ Dispatch domain events
            foreach (var domainEvent in domainEvents)
            {
                await _domainEventDispatcher.Dispatch(domainEvent);
            }

            // 4️⃣ Clear domain events
            foreach (var entry in ChangeTracker.Entries<AggregateRoot<Guid>>())
            {
                entry.Entity.ClearDomainEvents();
            }

            return result;
        }
    }
}
