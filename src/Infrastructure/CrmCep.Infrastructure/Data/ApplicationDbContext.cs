using CrmCep.Domain.Common;
using CrmCep.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmCep.Infrastructure.Data;

/// <summary>
/// Database context supporting EF Core Code First, soft deletion filters, and automatic audit updates.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.CustomerCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(c => c.CustomerCode)
                .IsUnique();

            entity.Property(c => c.FullName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(c => c.FullName);

            entity.Property(c => c.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired();

            entity.HasIndex(c => c.PhoneNumber);

            entity.Property(c => c.Email)
                .HasMaxLength(100);

            entity.Property(c => c.Address)
                .HasMaxLength(250);

            entity.Property(c => c.Notes)
                .HasMaxLength(1000);

            // Global query filter to exclude soft-deleted records from standard queries
            entity.HasQueryFilter(c => !c.IsDeleted);
        });

        // Configure User entity for JWT authentication
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.Property(u => u.FullName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(100);

            entity.Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
