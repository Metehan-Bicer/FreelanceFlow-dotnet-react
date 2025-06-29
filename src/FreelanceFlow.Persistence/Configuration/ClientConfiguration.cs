using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Domain.Entities;

namespace FreelanceFlow.Persistence.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.HasIndex(c => c.Email)
            .IsUnique();
            
        builder.Property(c => c.Phone)
            .HasMaxLength(50);
            
        builder.Property(c => c.Company)
            .HasMaxLength(200);
            
        builder.Property(c => c.TaxNumber)
            .HasMaxLength(50);
            
        builder.Property(c => c.Address)
            .HasMaxLength(500);
            
        builder.Property(c => c.Notes)
            .HasMaxLength(1000);
            
        // Relationships
        builder.HasMany(c => c.Projects)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}