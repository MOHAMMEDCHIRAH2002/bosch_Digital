using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Inventory;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PartNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.PartNumber)
            .IsUnique();

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.QuantityOnHand);

        builder.Property(s => s.CreatedOnUtc);
        builder.Property(s => s.CreatedBy).HasMaxLength(100);
        builder.Property(s => s.ModifiedOnUtc);
        builder.Property(s => s.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(s => s.Location);
    }
}