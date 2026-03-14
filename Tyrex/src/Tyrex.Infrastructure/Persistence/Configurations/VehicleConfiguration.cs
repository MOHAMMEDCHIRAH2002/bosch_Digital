using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Fleet;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Vin)
            .IsRequired()
            .HasMaxLength(17);

        builder.HasIndex(v => v.Vin)
            .IsUnique();

        builder.Property(v => v.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(v => v.LicensePlate);

        builder.Property(v => v.Make)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Year);

        builder.Property(v => v.CustomerId);

        builder.Property(v => v.IsInternalFleet);

        builder.HasIndex(v => v.CustomerId);

        builder.Property(v => v.CreatedOnUtc);
        builder.Property(v => v.CreatedBy).HasMaxLength(100);
        builder.Property(v => v.ModifiedOnUtc);
        builder.Property(v => v.ModifiedBy).HasMaxLength(100);
    }
}
