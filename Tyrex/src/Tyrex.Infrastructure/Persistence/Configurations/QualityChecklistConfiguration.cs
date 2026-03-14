using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Quality;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class QualityChecklistConfiguration : IEntityTypeConfiguration<QualityChecklist>
{
    public void Configure(EntityTypeBuilder<QualityChecklist> builder)
    {
        builder.ToTable("QualityChecklists");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.RepairOrderId);

        builder.HasIndex(q => q.RepairOrderId)
            .IsUnique();

        builder.Property(q => q.InspectorId);

        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(q => q.FinalNotes)
            .HasMaxLength(2000);

        builder.OwnsMany(q => q.Items, item =>
        {
            item.ToTable("QualityChecklistItems");
            item.HasKey(i => i.Id);
            item.WithOwner().HasForeignKey("QualityChecklistId");
            item.Property(i => i.Name).IsRequired().HasMaxLength(200);
            item.Property(i => i.Description).HasMaxLength(500);
            item.Property(i => i.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
            item.Property(i => i.Notes).HasMaxLength(1000);
        });

        builder.Property(q => q.CreatedOnUtc);
        builder.Property(q => q.CreatedBy).HasMaxLength(100);
        builder.Property(q => q.ModifiedOnUtc);
        builder.Property(q => q.ModifiedBy).HasMaxLength(100);
    }
}