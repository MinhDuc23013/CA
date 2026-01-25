using CA.Application.Abstractions;
using CA.Domain.Aggregates;
using CA.Domain.Base;
using CA.Infrastructures.EventBus;
using CA.Infrastructures.Logging;
using Microsoft.EntityFrameworkCore;

namespace CA.Infrastructures.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IInMemoryDomainEventBus _eventBus;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IInMemoryDomainEventBus eventBus)
            : base(options)
        {
            _eventBus = eventBus;
        }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        //public DbSet<Disbursement> Disbursements { get; set; }

        public DbSet<SystemExceptionLog> SystemExceptionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoanApplication>(entity =>
            {
                entity.ToTable("loan_applications");
                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.CIF)
                    .HasColumnName("cif")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.RequestedAmount)
                    .HasColumnName("requested_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(x => x.TermInMonths)
                    .HasColumnName("term_in_months")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()   // enum → string
                    .HasMaxLength(20)
                    .IsRequired();

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


            modelBuilder.Entity<SystemExceptionLog>(builder =>
            {
                builder.ToTable("system_exception_logs");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn(); // BIGSERIAL

                builder.Property(x => x.ApplicationName)
                    .HasColumnName("application_name")
                    .HasMaxLength(20)
                    .IsRequired();

                builder.Property(x => x.ServiceName)
                    .HasColumnName("service_name")
                    .HasMaxLength(50);

                builder.Property(x => x.Message)
                    .HasColumnName("message")
                    .IsRequired();

                builder.Property(x => x.RequestPath)
                    .HasColumnName("request_path")
                    .HasMaxLength(500);

                builder.Property(x => x.HttpMethod)
                    .HasColumnName("http_method")
                    .HasMaxLength(20);

                builder.Property(x => x.StatusCode)
                    .HasColumnName("status_code");

                builder.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();
            });

        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<Entity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            await _eventBus.PublishAsync(domainEvents);

            ChangeTracker
                .Entries<Entity>()
                .ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            return result;
        }

    }
}
