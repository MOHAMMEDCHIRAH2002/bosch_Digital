using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class DiagnosticConfiguration : IEntityTypeConfiguration<Diagnostic>
{
    public void Configure(EntityTypeBuilder<Diagnostic> builder)
    {
        builder.ToTable("Diagnostics");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.RepairOrderId);

        builder.HasIndex(d => d.RepairOrderId)
            .IsUnique();

        builder.Property(d => d.TechnicianId);

        builder.Property(d => d.Notes)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(d => d.TechnicalValidationState)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.MediaUrls)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(d => d.CreatedOnUtc);
        builder.Property(d => d.CreatedBy).HasMaxLength(100);
        builder.Property(d => d.ModifiedOnUtc);
        builder.Property(d => d.ModifiedBy).HasMaxLength(100);
    }
}