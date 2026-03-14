using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class RepairOrderConfiguration : IEntityTypeConfiguration<RepairOrder>
{
    public void Configure(EntityTypeBuilder<RepairOrder> builder)
    {
        builder.ToTable("RepairOrders");

        builder.HasKey(ro => ro.Id);

        builder.Property(ro => ro.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ro => ro.OrderNumber)
            .IsUnique();

        builder.Property(ro => ro.CustomerId);

        builder.Property(ro => ro.VehicleId);

        builder.Property(ro => ro.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ro => ro.VisitReason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ro => ro.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(ro => ro.Status);

        builder.Property(ro => ro.IntakePhotoUrls)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(ro => ro.CreatedOnUtc);
        builder.Property(ro => ro.CreatedBy).HasMaxLength(100);
        builder.Property(ro => ro.ModifiedOnUtc);
        builder.Property(ro => ro.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(ro => ro.CustomerId);
        builder.HasIndex(ro => ro.VehicleId);
        builder.HasIndex(ro => ro.CreatedOnUtc);
    }
}