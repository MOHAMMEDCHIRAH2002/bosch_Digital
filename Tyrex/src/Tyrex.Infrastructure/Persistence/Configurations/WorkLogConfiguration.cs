using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
    public void Configure(EntityTypeBuilder<WorkLog> builder)
    {
        builder.ToTable("WorkLogs");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.RepairOrderId);

        builder.HasIndex(w => w.RepairOrderId);

        builder.Property(w => w.TechnicianId);

        builder.Property(w => w.StartTime);

        builder.Property(w => w.EndTime);

        builder.Property(w => w.PauseReason)
            .HasMaxLength(500);

        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(w => w.Status);

        builder.Property(w => w.CreatedOnUtc);
        builder.Property(w => w.CreatedBy).HasMaxLength(100);
        builder.Property(w => w.ModifiedOnUtc);
        builder.Property(w => w.ModifiedBy).HasMaxLength(100);
    }
}