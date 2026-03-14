using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tyrex.Domain.Billing;

namespace Tyrex.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        builder.Property(i => i.RepairOrderId);

        builder.HasIndex(i => i.RepairOrderId)
            .IsUnique();

        builder.Property(i => i.DueDate);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(i => i.Status);

        builder.OwnsMany(i => i.Lines, line =>
        {
            line.ToTable("InvoiceLines");
            line.HasKey(l => l.Id);
            line.WithOwner().HasForeignKey("InvoiceId");
            line.Property(l => l.Description).IsRequired().HasMaxLength(500);
            line.Property(l => l.Quantity);
            line.Property(l => l.UnitPrice).HasPrecision(18, 2);
            line.Property(l => l.TaxRate).HasPrecision(18, 2);
        });

        builder.OwnsMany(i => i.Payments, payment =>
        {
            payment.ToTable("Payments");
            payment.HasKey(p => p.Id);
            payment.WithOwner().HasForeignKey("InvoiceId");
            payment.Property(p => p.Amount).HasPrecision(18, 2);
            payment.Property(p => p.PaymentDate);
            payment.Property(p => p.Method)
                .HasConversion<string>()
                .HasMaxLength(50);
            payment.Property(p => p.ReferenceInfo).HasMaxLength(200);
        });

        builder.Property(i => i.CreatedOnUtc);
        builder.Property(i => i.CreatedBy).HasMaxLength(100);
        builder.Property(i => i.ModifiedOnUtc);
        builder.Property(i => i.ModifiedBy).HasMaxLength(100);
    }
}