using IMS.Domain.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IMS.Domain.Products;
using IMS.SharedKernel;

namespace IMS.Infrastructure.JoinTables;

internal sealed class OrderProductsConfiguration : IEntityTypeConfiguration<OrderProducts>
{
    public void Configure(EntityTypeBuilder<OrderProducts> builder)
    {
        builder.ToTable("OrderProducts");

        builder.HasKey(op => new { op.OrderId, op.ProductId });

        builder.Property(op => op.OrderId)
            .IsRequired()
            .HasConversion(
                id => id,
                id => id);

        builder.Property(op => op.ProductId)
            .IsRequired()
            .HasConversion(
                id => id,
                id => id);

        builder.Property(op => op.Quantity)
            .IsRequired();

        builder.Property(op => op.Price)
            .HasPrecision(18, 4)
            .IsRequired()
            .HasConversion(
                price => price.Value,
                price => new Price(price));


        // Relationships

        builder
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId);

        builder
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId);

    }
}
