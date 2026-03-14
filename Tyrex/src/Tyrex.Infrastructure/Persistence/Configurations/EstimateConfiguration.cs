using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Commerce;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class EstimateConfiguration : IEntityTypeConfiguration<Estimate>
{
    public void Configure(EntityTypeBuilder<Estimate> builder)
    {
        builder.ToTable("Estimates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RepairOrderId);

        builder.HasIndex(e => e.RepairOrderId);

        builder.Property(e => e.Version);

        builder.HasIndex(e => new { e.RepairOrderId, e.Version })
            .IsUnique();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.ClientApprovalProofUrl)
            .HasMaxLength(500);

        builder.OwnsMany(e => e.Items, item =>
        {
            item.ToTable("EstimateLineItems");
            item.HasKey(i => i.Id);
            item.WithOwner().HasForeignKey("EstimateId");
            item.Property(i => i.Description).IsRequired().HasMaxLength(500);
            item.Property(i => i.Quantity);
            item.Property(i => i.UnitPrice).HasPrecision(18, 2);
            item.Property(i => i.TaxRate).HasPrecision(18, 2);
        });

        builder.Property(e => e.CreatedOnUtc);
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.ModifiedOnUtc);
        builder.Property(e => e.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(e => e.Status);
    }
}