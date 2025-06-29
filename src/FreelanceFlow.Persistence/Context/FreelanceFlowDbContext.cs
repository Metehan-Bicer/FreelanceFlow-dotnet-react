using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Domain.Entities;

namespace FreelanceFlow.Persistence.Context;

public class FreelanceFlowDbContext : DbContext
{
    public FreelanceFlowDbContext(DbContextOptions<FreelanceFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FreelanceFlowDbContext).Assembly);

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed sample data for development
        var clientId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Seed default user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = userId,
                Username = "admin",
                Email = "admin@freelanceflow.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "System",
                LastName = "Administrator",
                Role = FreelanceFlow.Domain.Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = clientId,
                Name = "Örnek Şirket Ltd.",
                Email = "info@ornekfirma.com",
                Phone = "+90 212 555 0123",
                Company = "Örnek Şirket Ltd.",
                Address = "İstanbul, Türkiye",
                TaxNumber = "1234567890",
                Status = FreelanceFlow.Domain.Enums.ClientStatus.Active,
                Notes = "İlk müşteri kaydı",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Project>().HasData(
            new Project
            {
                Id = projectId,
                Name = "E-Ticaret Web Sitesi",
                Description = "Modern e-ticaret platformu geliştirme projesi",
                ClientId = clientId,
                StartDate = DateTime.UtcNow,
                DeadlineDate = DateTime.UtcNow.AddMonths(3),
                Budget = 50000m,
                Status = FreelanceFlow.Domain.Enums.ProjectStatus.InProgress,
                Priority = FreelanceFlow.Domain.Enums.Priority.High,
                ProgressPercentage = 25,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}